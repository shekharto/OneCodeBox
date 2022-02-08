export class SortingParameter {
  sortColumn: string;
  direction: string;

  constructor(sortColumn: string, direction: string) {
    this.sortColumn = sortColumn;
    this.direction = direction;
  }
}
