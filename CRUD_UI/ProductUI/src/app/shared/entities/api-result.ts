
export class ApiResult<T> {
  isException: boolean;
  isValidationError: boolean;
  isSuccess: boolean;
  message: string;
  milliseconds: number;
  processTime: string;
  transactionId: string;
  resultArray: T[];

  getData(): T[] {
    return this.resultArray ? this.resultArray : new Array<T>(0);
  }
}
