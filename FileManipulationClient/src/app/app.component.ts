import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import {
  FileInfo,
  RemovingEventArgs,
  SelectedEventArgs,
  SuccessEventArgs,
  UploaderComponent,
} from '@syncfusion/ej2-angular-inputs';
import { AppService } from './service/app.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'FileManipulationClient';
  //maxFileSize: number = 104857600; //100Mb
  maxFileSize: number = 52428800; //50Mb
  imageFormats: string = '.png, .jpg, .jpeg, .gif';
  fileNameArray: string[] = [];
  @ViewChild('defaultupload')
  uploadObj: UploaderComponent;
  path: Object = {
    saveUrl: 'https://localhost:7150/File/Save',
    removeUrl: 'https://localhost:7150/File/Remove',
  };

  constructor(private readonly appService: AppService) {}

  ngOnDestroy(): void {
    this.appService.cleanUp(this.fileNameArray).subscribe((data) => {
      if (data) {
        console.log(data);
      }
    });
  }

  ngOnInit() {}

  onUploadSuccess(args: SuccessEventArgs): void {
    if (args.operation === 'upload') {
      if (!this.fileNameArray.includes(args.file.name)) {
        this.fileNameArray.push(args.file?.name);
      }
    } else if (args.operation === 'remove') {
    }
  }

  onUploadFailure(args: any): void {
    if (args.file.name && this.fileNameArray.includes(args.file.name)) {
      this.fileNameArray = this.fileNameArray.filter((del) => {
        del !== args.file.name;
      });
    }
  }

  onFileSelected(args: SelectedEventArgs): void {
    if (args.filesData && args.filesData.length > 1) {
      args.filesData.forEach((fileName) => {
        this.fileNameArray.push(fileName.name);
      });
    }
    // Filter the 15 files only to showcase
    args.filesData.splice(15);
    let filesData: FileInfo[] = this.uploadObj.getFilesData();
    let allFiles: FileInfo[] = filesData.concat(args.filesData);
    if (allFiles.length > 15) {
      for (let i: number = 0; i < allFiles.length; i++) {
        if (allFiles.length > 15) {
          allFiles.shift();
        }
      }
      args.filesData = allFiles;
      // set the modified custom data
      args.modifiedFilesData = args.filesData;
    }
    args.isModified = true;
  }

  onFileRemove(args: RemovingEventArgs): void {
    if (
      this.fileNameArray &&
      this.fileNameArray.length > 1 &&
      args.filesData &&
      args.filesData[0].name
    ) {
      this.fileNameArray.forEach((ele, index) => {
        if (ele === args.filesData[0].name) {
          this.fileNameArray.splice(index, 1);
        }
      });
    }
  }

  onDownload() {
    this.appService.downloadPdf(this.fileNameArray).subscribe((pdf) => {
      if (pdf) {
        var link = document.createElement('a');
        var URL = window.URL || window.webkitURL;
        var downloadUrl = URL.createObjectURL(pdf);
        link.href = downloadUrl;
        link.download = 'test.pdf';
        link.dispatchEvent(
          new MouseEvent('click', {
            bubbles: true,
            cancelable: true,
            view: window,
          })
        );
      }
    });
  }
}
