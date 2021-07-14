import { Component, OnInit } from '@angular/core';
import { CommonModule } from "@angular/common";

import {UserAccessService} from "../../../../Shared/services/user-access.service"
import {UserTaskType} from "../../../../Shared/enums/user-task-type";
import {ActivityType, moduleType} from "../../../../Shared/enums/enums";

@Component({
  selector: 'app-measures',
  templateUrl: './measures.component.html',
  styleUrls: ['./measures.component.scss']
})
export class MeasuresComponent implements OnInit {
  canCreate: boolean;
  canManage: boolean;
  canView: boolean;
  canViewSql: boolean;

  constructor(private userAccessService: UserAccessService) { }

  ngOnInit(): void {
    this.configureAccess();

  }

   configureAccess(): void {
    // this will enable or disable button/functionality as per the rights given.
     this.canManage = this.userAccessService.hasRole(UserTaskType.MeasureManage);
     this.canCreate =  this.canManage || this.userAccessService.hasRole(UserTaskType.MeasureCreate);
     this.canView = this.canCreate || this.canManage || this.userAccessService.hasRole(UserTaskType.MeasureView);

     this.canViewSql = this.canExecuteItem(ActivityType.Sql);
   }


  private canExecuteItem(taskType: ActivityType): boolean {
    if (!this.userAccessService.hasRights(moduleType.measure, taskType)) {
      return false;
    } else {
    return true;
    }
  }
}
