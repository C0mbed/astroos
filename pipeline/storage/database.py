"""Frame metadata database — SQLite backend."""

from __future__ import annotations

import sqlite3
from pathlib import Path
from typing import Any

SCHEMA = """
CREATE TABLE IF NOT EXISTS frames (
    id                  TEXT PRIMARY KEY,
    path                TEXT NOT NULL,
    checksum            TEXT,
    target              TEXT,
    filter              TEXT,
    exposure_s          REAL,
    gain                INTEGER,
    binning             TEXT,
    camera_id           TEXT,
    date_obs            TEXT,
    ra_deg              REAL,
    dec_deg             REAL,
    fwhm_arcsec         REAL,
    eccentricity        REAL,
    snr                 REAL,
    star_count          INTEGER,
    background_adu      REAL,
    background_gradient REAL,
    clipping_fraction   REAL,
    temp_sensor_c       REAL,
    state               TEXT,
    rejection_reason    TEXT,
    created_at          TEXT DEFAULT (datetime('now'))
);

CREATE INDEX IF NOT EXISTS idx_frames_target ON frames(target);
CREATE INDEX IF NOT EXISTS idx_frames_filter ON frames(filter);
CREATE INDEX IF NOT EXISTS idx_frames_state  ON frames(state);
CREATE INDEX IF NOT EXISTS idx_frames_date   ON frames(date_obs);
"""


class FrameDatabase:
    def __init__(self, db_path: Path):
        self._path = db_path
        self._path.parent.mkdir(parents=True, exist_ok=True)
        self._conn = sqlite3.connect(str(self._path), check_same_thread=False)
        self._conn.row_factory = sqlite3.Row
        self._conn.executescript(SCHEMA)
        self._conn.commit()

    def upsert_frame(self, frame_dict: dict[str, Any]) -> None:
        cols = [
            "id", "path", "checksum", "target", "filter", "exposure_s", "gain",
            "binning", "camera_id", "date_obs", "ra_deg", "dec_deg",
            "fwhm_arcsec", "eccentricity", "snr", "star_count",
            "background_adu", "background_gradient", "clipping_fraction",
            "temp_sensor_c", "state", "rejection_reason",
        ]
        placeholders = ", ".join("?" for _ in cols)
        col_names = ", ".join(cols)
        updates = ", ".join(f"{c}=excluded.{c}" for c in cols if c != "id")
        values = [frame_dict.get(c) for c in cols]
        self._conn.execute(
            f"INSERT INTO frames ({col_names}) VALUES ({placeholders}) "
            f"ON CONFLICT(id) DO UPDATE SET {updates}",
            values,
        )
        self._conn.commit()

    def query_frames(self, **filters) -> list[sqlite3.Row]:
        where_clauses = []
        values = []
        for k, v in filters.items():
            where_clauses.append(f"{k} = ?")
            values.append(v)
        where = ("WHERE " + " AND ".join(where_clauses)) if where_clauses else ""
        return self._conn.execute(
            f"SELECT * FROM frames {where} ORDER BY date_obs DESC", values
        ).fetchall()

    def close(self) -> None:
        self._conn.close()
