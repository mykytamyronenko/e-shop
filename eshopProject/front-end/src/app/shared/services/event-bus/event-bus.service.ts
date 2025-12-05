import { Injectable } from '@angular/core';
import {EventBus} from './event-bus';
import {filter, Observable, Subject} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EventBusService {
  private _bus: Subject<EventBus> = new Subject();

  constructor() { }

  publish(event: EventBus) {
    this._bus.next(event);
  }

  listen(eventName: string): Observable<EventBus> {
    return this._bus.asObservable().pipe(
      filter(e => e.name == eventName),
    );
  }
}
