import {Subscription} from "rxjs";

export interface IEvent<T>  {
  publish(payload: T): void;

  subscribe(callback: (payload: T) => void): Subscription;
}
