import {Injectable} from "@angular/core";
import {SortingParameter} from "../entities/SortingParameter";

@Injectable({providedIn: "root"})
export class SortingService {
  sortingParameter: SortingParameter[] = [];
sorting
  constructor() {
  }

  onSortData(column: string, data: any[], decode?: (object: any, field: string, direction: string) => any): any[] {
    let target = this.sortingParameter.find(s => s.sortColumn === column);
    if (target) {
       this.sortingParameter = this.sortingParameter.filter(s => s.sortColumn !== target.sortColumn);
    } else {
      target = {sortColumn: column, direction: "none"};
    }
    target.direction = this.setDirection(target.direction);

    if (target.direction !== "none") {
      this.sortingParameter.push(target);
    }

    return this.applySort(data, decode);

  return data;
  }

  applySort(inputArray: any[], decode?: (object: any, field: string, direction: string) => any): any[] {
    if (this.sortingParameter && this.sortingParameter.length > 0) {
      inputArray.sort((first, second) => {
        let result = 0;
        for(let parameter of this.sortingParameter) {
            let firstField = decode ? decode(first, parameter.sortColumn, parameter.direction) : this.getFieldType(first[parameter.sortColumn]);
            let secondField = decode ? decode(second, parameter.sortColumn, parameter.direction) : this.getFieldType(first[parameter.sortColumn]);
            ///////////////
            // this is the short and better way
            result = this.compare(firstField, secondField, parameter.direction === "asc");
            //////////////

/*            if (typeof firstField !== typeof secondField) {
              if (firstField === undefined && secondField === undefined) {
                result = 0;
              } else if (firstField === undefined) {
                result = parameter.direction === "asc" ? 1 : -1
              }
              if (secondField === undefined) {
                result = parameter.direction === "asc" ? 1 : -1
              }
            } else {
              if (firstField < secondField) {
                result = parameter.direction === "asc" ? -1 : 1;
              } else if (firstField > secondField) {
                result = parameter.direction === "asc" ? 1 : -1;
              } else {
                result = 0;
              }
            }*/
            if (result !== 0) {
              break;
            }
        }
        return result;
      });
    }
    return inputArray;
  }

  private getFieldType(value: any): any {
    if (value !== undefined) {
      switch (typeof value) {
        case "string":
          return value.toString().toLowerCase();
        case "number":
          return value;
        case "boolean":
          return value;
        default:
          return value;
      }
    }
  }


  private setDirection(direction: string): string {
    switch (direction) {
      case "none":
        return "asc";
      case "asc":
        return "desc";
      case "desc":
        return "none";
    }
    return "none";
  }


  private compare(a: number | string | boolean, b: number | string | boolean, isAsc: boolean) {
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

}
