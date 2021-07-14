
export enum ObjectStateType {
  Unchanged = "Unchanged",
  Added = "Added",
  Modified = "Modified",
  Deleted = "Deleted"
}

export enum moduleType {
  patient = "Patient",
  measure = "Measure"
}

export enum UserRoleNames {
  Administrator = "Administrator"
}

export enum ActivityType {
  Activate,
  Audit,
  Build,
  Clear,
  Create,
  Delete,
  Edit,
  Manage,
  Move,
  PatientList,
  View,
  WhereUsed,
  MeasureList,
  Sql
}
