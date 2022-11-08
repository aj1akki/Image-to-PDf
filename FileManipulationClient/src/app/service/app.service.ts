import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AppService {
  private readonly apiUrlDownload = 'https://localhost:7150/File/Download';
  private readonly apiUrlCleanUp = 'https://localhost:7150/File/CleanUp';
  private readonly httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
  };
  constructor(private readonly httpClient: HttpClient) {}

  downloadPdf(imageList: any): Observable<Blob> {
    return this.httpClient
      .post<Blob>(this.apiUrlDownload, imageList, {
        headers: this.httpOptions.headers,
        responseType: 'blob' as 'json',
      })
      .pipe(
        map((data) => {
          //You can perform some transformation here
          return data;
        }),
        catchError((err) => {
          alert('Pdf cannot be downloaded due to server issue');
          throw err;
        })
      );
  }

  cleanUp(imageList: any): Observable<any> {
    return this.httpClient
      .post<any>(this.apiUrlCleanUp, imageList, this.httpOptions)
      .pipe(
        (map((data) => {
          return data;
        }),
        catchError((err) => {
          throw err;
        }))
      );
  }
}
