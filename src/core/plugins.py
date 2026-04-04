"""Plugin registry — discovers and loads hardware drivers and pipeline extensions."""

from __future__ import annotations

import importlib
import importlib.metadata
import logging
from typing import Any, Type

log = logging.getLogger(__name__)

# Entry point groups for each plugin category
GROUPS = {
    "mount": "astroos.drivers.mount",
    "camera": "astroos.drivers.camera",
    "focuser": "astroos.drivers.focuser",
    "filter_wheel": "astroos.drivers.filter_wheel",
    "rotator": "astroos.drivers.rotator",
    "dome": "astroos.drivers.dome",
    "weather": "astroos.drivers.weather",
    "pipeline_stage": "astroos.pipeline.stages",
    "scheduler_constraint": "astroos.scheduler.constraints",
}


class PluginRegistry:
    def __init__(self):
        self._plugins: dict[str, dict[str, Type]] = {g: {} for g in GROUPS}

    def discover(self) -> None:
        """Load all plugins from installed package entry points."""
        for category, group in GROUPS.items():
            try:
                eps = importlib.metadata.entry_points(group=group)
            except Exception:
                continue
            for ep in eps:
                try:
                    cls = ep.load()
                    self._plugins[category][ep.name] = cls
                    log.debug("Plugin loaded: [%s] %s → %s", category, ep.name, cls)
                except Exception:
                    log.exception("Plugin load failed: [%s] %s", category, ep.name)

    def register(self, category: str, name: str, cls: Type) -> None:
        """Register a plugin class directly (for testing or inline drivers)."""
        if category not in self._plugins:
            raise ValueError(f"Unknown plugin category: {category}")
        self._plugins[category][name] = cls

    def get(self, category: str, name: str) -> Type:
        """Return the class for a named plugin."""
        try:
            return self._plugins[category][name]
        except KeyError:
            available = list(self._plugins.get(category, {}).keys())
            raise KeyError(
                f"No plugin '{name}' in category '{category}'. "
                f"Available: {available}"
            )

    def list(self, category: str) -> list[str]:
        return list(self._plugins.get(category, {}).keys())
