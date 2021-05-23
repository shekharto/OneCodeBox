import {AfterViewInit, ChangeDetectorRef, ElementRef, Injectable, Injector, OnChanges, OnDestroy, SimpleChanges} from "@angular/core";
import {Subscription} from "rxjs";



@Injectable()
export abstract class ComponentBase implements OnDestroy {
  private subscribers: Subscription[] = [];


  protected constructor(protected self?: ElementRef, injector?: Injector) {
  }


  ngOnDestroy(): void {
    this.subscribers.forEach(value => {
      if (value) {
        value.unsubscribe();
      }
    });
  }

  protected registerSubscriber(item: Subscription): void {
    this.subscribers.push(item);
  }

}
