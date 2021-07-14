import {Injectable} from "@angular/core";

import {AuthenticationService} from "../../app/jwt-security/repositories-services/authentication.service"
import {ActivityType, moduleType, UserRoleNames} from "../enums/enums";
import {UserTaskType} from "../enums/user-task-type";
import {Sorting} from "../helper/Sorting";


@Injectable({providedIn: "root"})
export class UserAccessService {
  private userRoles: string[];
  private roles: Map<UserTaskType, boolean> = new Map<UserTaskType, boolean>();
  private rights: Map<moduleType, ActivityType[]> = new Map<moduleType, ActivityType[]>();

    constructor(private authenticationService: AuthenticationService) {
      this.getApplicationTasks();
      this.getUserRoles();
   }

  hasAdminRole(): boolean {
    return this.userRoles.includes(UserRoleNames.Administrator);
  }

  private getApplicationTasks(): void {
    let userTasks = this.authenticationService.getUserTaskIds();
    this.setUserTasks(userTasks);
  }

  hasRole(role: UserTaskType): boolean {
    if (this.hasAdminRole()) { return true; }
    return this.roles.has(role);
  }

  private getUserRoles(): void {
    this.userRoles = this.authenticationService.getUserRoleNames();
  }

  hasRights(module: moduleType, activity: ActivityType): boolean {
    if (this.hasAdminRole()) { return true; }
    return this.rights.has(module) && this.rights.get(module).some(a => a === activity);
  }

  private getUserTasks(userTask: UserTaskType): ActivityType[] {
      switch (userTask) {
        case UserTaskType.MeasureView:
          return [
            ActivityType.MeasureList,
            ActivityType.Audit,
            ActivityType.WhereUsed,
            ActivityType.Sql
          ];
        case UserTaskType.MeasureCreate:
          return [
            ActivityType.MeasureList,
            ActivityType.Audit,
            ActivityType.WhereUsed,
            ActivityType.Create,
            ActivityType.Sql
          ];
        case UserTaskType.MeasureManage:
          return [
            ActivityType.MeasureList,
            ActivityType.Audit,
            ActivityType.WhereUsed,
            ActivityType.Manage,
            ActivityType.Sql
          ];
        case UserTaskType.PatientView:
          return [
            ActivityType.PatientList,
            ActivityType.Audit,
            ActivityType.WhereUsed,
            ActivityType.Sql
          ];
        case UserTaskType.PatinetCreate:
          return [
            ActivityType.PatientList,
            ActivityType.Audit,
            ActivityType.WhereUsed,
            ActivityType.Create,
            ActivityType.Sql
          ];
        case UserTaskType.PatientManage:
          return [
            ActivityType.PatientList,
            ActivityType.Audit,
            ActivityType.WhereUsed,
            ActivityType.Manage,
            ActivityType.Sql
          ];
        default:
          return [];
      }
  }

  private combineRights(module: moduleType, type: UserTaskType): void {
      let tasks = this.getUserTasks(type);
      if (this.rights.has(module))  {
        tasks = Sorting.distinct(tasks.concat(this.rights.get(module)));
      }
      this.rights.set(module, tasks);
  }

  private setUserTasks(userTasks: number[]): void {
      if (userTasks) {
            userTasks.forEach(task => {
              let type: UserTaskType = task;
              this.roles.set(type, true);
              switch (type) {
                case UserTaskType.MeasureCreate:
                case UserTaskType.MeasureManage:
                case UserTaskType.MeasureView:
                  this.combineRights(moduleType.measure, type);
                  break;
                case UserTaskType.PatinetCreate:
                case UserTaskType.PatientManage:
                case UserTaskType.MeasureView:
                  this.combineRights(moduleType.patient, type);
                  break;
              }
            });
      }
  }


}
