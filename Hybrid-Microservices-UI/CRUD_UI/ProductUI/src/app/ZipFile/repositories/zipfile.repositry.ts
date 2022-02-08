import {EventEmitter, Injectable, Injector} from "@angular/core";
import { catchError, map, tap } from 'rxjs/operators';

import {ApiResult} from "../../shared/entities/api-result";
import {ZipFileService} from "../services/zipfile.service";
import {BaseRepository} from "../../shared/repositories/base.repository";

@Injectable({providedIn: "root"})
export class ZipFileRepository  extends BaseRepository {

  constructor(injector: Injector, private service: ZipFileService) {
    super(injector);
  }

  exportToXmlInZip(fileType: string, zipSize: number): Promise<any[]> {
    return this.getMultiple<any>(this.service, this.service.exportToXmlInZip.bind(this.service, fileType, zipSize));
  }
}
