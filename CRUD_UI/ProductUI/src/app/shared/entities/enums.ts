
export enum ObjectStateType {
  Unchanged = "Unchanged",
  Added = "Added",
  Modified = "Modified",
  Deleted = "Deleted"
}

export enum ActionType {
  Get = "Getting from database",
  Post = "Posting to database",
  Delete = "Deleting from database",
  Communicate = "Database Access"
}

export enum EventType {
  About,
  LoadMainGrid,
  NewProductAdded,
  ProductUpdated
}

export enum ModuleType {
  Unspecified = "Unspecified",
  Product = "Product"
}

export enum transactionType {
   add = "Add",
   update = "Update",
   delete = "Delete"
}
