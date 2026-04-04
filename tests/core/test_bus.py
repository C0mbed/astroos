"""Tests for the event bus."""

from src.core.bus import EventBus, Event


def test_emit_and_receive():
    bus = EventBus()
    received: list[Event] = []
    bus.subscribe("test.event", received.append)
    bus.emit("test.event", source="test", payload={"x": 1})
    assert len(received) == 1
    assert received[0].type == "test.event"
    assert received[0].payload["x"] == 1


def test_wildcard_subscription():
    bus = EventBus()
    all_events: list[Event] = []
    bus.subscribe("*", all_events.append)
    bus.emit("a.b", source="x", payload={})
    bus.emit("c.d", source="y", payload={})
    assert len(all_events) == 2


def test_unsubscribe():
    bus = EventBus()
    received: list[Event] = []
    bus.subscribe("test.event", received.append)
    bus.unsubscribe("test.event", received.append)
    bus.emit("test.event", source="test", payload={})
    assert len(received) == 0


def test_handler_error_does_not_propagate():
    bus = EventBus()

    def bad_handler(event: Event) -> None:
        raise RuntimeError("oops")

    bus.subscribe("test.event", bad_handler)
    # Should not raise
    bus.emit("test.event", source="test", payload={})
