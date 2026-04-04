"""Configuration loader — YAML files with environment variable overrides."""

from __future__ import annotations

import os
from pathlib import Path
from typing import Any

import yaml


class Config:
    """
    Hierarchical config loaded from YAML, with env var overrides.

    Env vars override using double-underscore as path separator:
        ASTROOS__HARDWARE__MOUNT__DRIVER=indi_mount
        overrides config["hardware"]["mount"]["driver"]
    """

    ENV_PREFIX = "ASTROOS"

    def __init__(self, data: dict):
        self._data = data
        self._apply_env_overrides()

    @classmethod
    def from_file(cls, path: Path) -> "Config":
        with open(path) as f:
            data = yaml.safe_load(f) or {}
        return cls(data)

    @classmethod
    def from_files(cls, *paths: Path) -> "Config":
        merged: dict = {}
        for path in paths:
            if path.exists():
                with open(path) as f:
                    layer = yaml.safe_load(f) or {}
                _deep_merge(merged, layer)
        return cls(merged)

    def get(self, *keys: str, default: Any = None) -> Any:
        node = self._data
        for key in keys:
            if not isinstance(node, dict) or key not in node:
                return default
            node = node[key]
        return node

    def __getitem__(self, key: str) -> Any:
        return self._data[key]

    def _apply_env_overrides(self) -> None:
        prefix = self.ENV_PREFIX + "__"
        for key, value in os.environ.items():
            if not key.startswith(prefix):
                continue
            parts = key[len(prefix):].lower().split("__")
            node = self._data
            for part in parts[:-1]:
                node = node.setdefault(part, {})
            node[parts[-1]] = value


def _deep_merge(base: dict, override: dict) -> None:
    for key, value in override.items():
        if key in base and isinstance(base[key], dict) and isinstance(value, dict):
            _deep_merge(base[key], value)
        else:
            base[key] = value
