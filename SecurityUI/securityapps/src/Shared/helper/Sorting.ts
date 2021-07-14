export class Sorting {

  static greaterThan(first: any, second: any): number {
    if (first && second) {
      let type = typeof first;
      switch (type) {
        case "number" :
          return first - second;
        case "string":
          return first.toLowerCase() < second.toLowerCase() ? -1 : first.toLowerCase() > second.toLowerCase() ? 1 : 0;
        case "boolean" :
          return !first && second ? -1 : first && !second ? 1 : 0;
        default:
          return 0;
      }
    }
    return 0;
  }

  static distinct(values: any[]): any[] {
    return values.filter((x, i, a) => a.indexOf(x) === i);
  }
}
