import {Subject, Subscription} from "rxjs";
import {IEvent} from "../interfaces/interfaces";

export class Event<T> implements IEvent<T> {
  private subject = new Subject<T>();

  constructor() {
  }

  publish(payload: T): void {
    this.subject.next(payload);
  }

  subscribe(callback: (payload: T) => void): Subscription {
    return this.subject.asObservable().subscribe(callback);
  }

}
