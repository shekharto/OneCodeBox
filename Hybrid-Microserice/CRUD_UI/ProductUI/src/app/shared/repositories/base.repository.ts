
import {ApiResult} from "../entities/api-result";
import {EventEmitter, Injector} from "@angular/core";
import {EventService} from "../service/event.service";
import {Observable} from "rxjs";
import {catchError} from "rxjs/operators";
import {EventType, ModuleType} from "../entities/enums";


export abstract class BaseRepository {
  protected eventService: EventService;

  protected constructor(injector: Injector) {
    this.eventService = injector.get(EventService);
  }

  protected async getMultiple<T>(service: any, callback: (param?: any) => Promise<ApiResult<T>>,   args?: RepositoryParameters): Promise<T[]> {
    return this.handleMultiple<T>(service, callback,  args);
  }

  protected async getSingle<T>(service: any, callback: (param: any) => Promise<ApiResult<T>>,   args?: RepositoryParameters): Promise<T> {
    return this.handleSingle<T>(service, callback, args);
  }


  protected async deleteSingle<T>(service: any, callback: (param: any) => Promise<ApiResult<boolean>>, args?: RepositoryParameters): Promise<boolean> {
    return this.handleDelete<boolean>(service, callback, args);
  }


  /* Helper Methods */


  private async handleDelete<T>(service: any, callback: (param: any) => Promise<ApiResult<T>>,  args?: RepositoryParameters): Promise<boolean> {

    return new Promise<boolean>((resolve, reject) => {
      callback.call(service).then(result => {

        if (result.isSuccess) {
          if (args) {
            this.handleEventEmitters(args.events, result.resultArray[0]);
           // this.handleEventTypes(args.eventTypes, result.resultArray[0]);
          }
          resolve(true);
        } else {
           reject(result.message);
        }
      }).catch((error) => {
        catchError(error.message);
      });
    });
  }


  private async handleSingle<T>(service: any, callback: (param: any) => Promise<ApiResult<T>>,    args?: RepositoryParameters): Promise<T> {

    return new Promise<T>((resolve, reject) => {
      callback.call(service).then(result => {
        if (result.isSuccess) {
          if (args) {
            this.handleEventEmitters(args.events, result.resultArray[0]);
            // this.handleEventTypes(args.eventTypes, result.resultArray[0]);
          }
          resolve(result.resultArray[0]);
        } else {
           reject(result.message);
        }
      }).catch((error) => {
        catchError(error.message);
      });
    });
  }

  private handleMultiple<T>(service: any, callback: (param?: any) => Promise<ApiResult<T>>,  args?: RepositoryParameters): Promise<T[]> {
    return new Promise<T[]>((resolve, reject) => {
      callback.call(service).then(result => {

        if (result.isSuccess) {

          if (args) {
            this.handleEventEmitters(args.events, result.resultArray);
           // this.handleEventTypes(args.eventTypes, result.resultArray);
          }

          resolve(result.resultArray);
        } else {
          reject(result.message);
        }
      }).catch((error) => {
        catchError(error)
      });
    });
  }

  private handleEventEmitters(events: EventEmitter<any>[] | undefined, resultArray: any) {
    if (events && events.length > 0) {
      events.forEach(event => {
        event.emit(resultArray);
      });
    }
  }


}


export class RepositoryParameters {
  constructor(public eventTypes?: { eventType: EventType; arg?: ModuleType }[], public events?: EventEmitter<any>[]) {
  }
}
