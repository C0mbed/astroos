"""Event bus — synchronous pub/sub for inter-component communication."""

from __future__ import annotations

import logging
import time
import uuid
from collections import defaultdict
from dataclasses import dataclass, field
from typing import Callable

log = logging.getLogger(__name__)


@dataclass
class Event:
    type: str
    source: str
    payload: dict
    session_id: str = ""
    id: str = field(default_factory=lambda: str(uuid.uuid4()))
    timestamp: float = field(default_factory=time.time)


Handler = Callable[[Event], None]


class EventBus:
    """
    Simple synchronous event bus.

    Handlers are called in registration order, synchronously, in the caller's thread.
    Use subscribe("*") to receive all events.
    """

    def __init__(self):
        self._handlers: dict[str, list[Handler]] = defaultdict(list)

    def subscribe(self, event_type: str, handler: Handler) -> None:
        self._handlers[event_type].append(handler)

    def unsubscribe(self, event_type: str, handler: Handler) -> None:
        self._handlers[event_type].remove(handler)

    def emit(self, event_type: str, source: str, payload: dict, session_id: str = "") -> Event:
        event = Event(
            type=event_type,
            source=source,
            payload=payload,
            session_id=session_id,
        )
        for handler in self._handlers.get(event_type, []):
            try:
                handler(event)
            except Exception:
                log.exception("EventBus: handler error for event %s", event_type)
        for handler in self._handlers.get("*", []):
            try:
                handler(event)
            except Exception:
                log.exception("EventBus: wildcard handler error for event %s", event_type)
        return event
