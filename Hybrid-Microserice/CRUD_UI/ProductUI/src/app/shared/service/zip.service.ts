import {Injectable} from "@angular/core";
import * as FileSaver from "file-saver";
import {FileExtensionType} from "../enums/file-extension-type";

@Injectable({providedIn: "root"})
export class ZipService {
  constructor() {
  }

  exportByFileExtension(buffer: any, fileExtensionType: FileExtensionType) {
    if (!!buffer) {
      const blob = ZipService.decodeToString(buffer, "application/octet-stream", 512);
      if (!!blob) {
        //  FileSaver.saveAs(blob, `newFile_${new Date().getTime()}${fileExtensionType}`);
           FileSaver.saveAs(blob, `newFile_${new Date().getTime()}${FileExtensionType.Zip}`);
      } else { console.log("no data found"); }
    }
  }

  //  actual conversion function (complex)
  private static decodeToString(b64Data, contentType, sliceSize): Blob {
    contentType = contentType || "";
    sliceSize = sliceSize || 512;
    const byteCharacters = atob(b64Data); // this Decodes a base64 encoded string like c#  Convert.FromBase64String()
    const byteArrays = [];

    for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
      const slice = byteCharacters.slice(offset, offset + sliceSize);

      const byteNumbers = new Array(slice.length);
      for (let i = 0; i < slice.length; i++) {
        byteNumbers[i] = slice.charCodeAt(i);
      }
      const byteArray = new Uint8Array(byteNumbers);
      byteArrays.push(byteArray);
    }
    // conver byte to blob type
    return new Blob(byteArrays, {type: contentType});
  }

}
