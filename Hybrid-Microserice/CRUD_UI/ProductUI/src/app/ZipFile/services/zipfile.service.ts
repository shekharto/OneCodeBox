import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {ApiResult} from "../../shared/entities/api-result";
import {BaseService} from "../../shared/service/base.service";

@Injectable({providedIn: "root"})
export class ZipFileService extends BaseService {
  url: string;

  constructor(private httpClient: HttpClient) {
    super();
    this.url = this.getApiUrl();
  }

  exportToXmlInZip(fileType: string, zipSize: number): Promise<ApiResult<any>> {
    let apiUrl = `${this.url}/Export/Id/${zipSize}/FileType/${fileType}`;
    return this.httpClient.get<ApiResult<any>>(apiUrl).toPromise();
  }
}
