import {Injectable} from "@angular/core";
import {IEvent} from "../interfaces/interfaces";
import {EventType} from "../entities/enums";
import {Event} from "../entities/event";

@Injectable({providedIn: "root"})
export class EventService {
  private events: Map<EventType, IEvent<any>>;

  constructor() {
    this.events = new Map<EventType, IEvent<any>>();
  }

  getNotifier<T>(type: EventType): IEvent<T> {
    if (!this.events.has(type)) {
      this.events.set(type, new Event<T>());
    }
    return this.events.get(type);
  }
}
