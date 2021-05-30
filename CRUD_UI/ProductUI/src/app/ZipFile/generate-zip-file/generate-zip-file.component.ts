import { Component, OnInit } from '@angular/core';
import {ZipFileRepository} from "../repositories/zipfile.repositry";
import {FileExtensionType} from "../../shared/enums/file-extension-type";
import {ZipService} from "../../shared/service/zip.service";


@Component({
  selector: 'app-generate-zip-file',
  templateUrl: './generate-zip-file.component.html',
  styleUrls: ['./generate-zip-file.component.scss']
})
export class GenerateZipFileComponent implements OnInit {

  constructor(private zipFileRepository: ZipFileRepository,
              private zipService: ZipService) { }

  ngOnInit(): void {
  }

  generateXmlZipFile(): void {
   // send file type and set max size of zip file (example : 1mb  (1 * 1024))

    /// Overall this function (implented in api) read the compressed xml file and decompress it. If the assigned zip file size
    /// is exceed (imaxQRDAFileSizeMB) then it will create individual zip file  as per size and club it into single zip file.
    /// so in one single zip file you will find another zip files as per size mention.
    /// There UI will the data in byte array... and generate a single zip file
    /// Else, the final zip file contain plain xml files.
    this.zipFileRepository.exportToXmlInZip("xml", 1)
      .then(data => {
        if (!!data) {
          this.zipService.exportByFileExtension(data,  FileExtensionType.Xml);
        }
      });
  }

}
