import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {Observable, Subject} from "rxjs";
import {catchError, retryWhen, scan, timeout} from "rxjs/operators";
import {ApiResult} from "../entities/api-result";

export abstract class BaseService {

  protected constructor() {

  }

  protected handleHttp<T>(method: Observable<Object>, timeoutvalue?: number): Observable<ApiResult<T>> {
    let message: string;

    return new Observable(observable => {
        method
          .pipe(catchError((error: HttpErrorResponse) => {
              if (error.status === 0) {
                message = "Server does not support requested HTTP verb";
              } else {
                message = error.message;
              }
              throw error;
            }),
            retryWhen(error => {
              return error.pipe(scan((accumulator) => {
                if (accumulator > 3 || message) {
                  throw error;
                } else {
                  return accumulator + 1;
                }
              }, 1));
            }), timeout(timeoutvalue || 300000))
          .subscribe((data) => {
              observable.next(Object.assign(new ApiResult<T>(), data));
            },
            () => {
              const result = new ApiResult<T>();
              result.isSuccess = false;
              result.resultArray = [];
              result.message = message;
              observable.next(result);
              observable.complete();
            },
            () => {
              observable.complete();
            });
      }
    );
  }


}
