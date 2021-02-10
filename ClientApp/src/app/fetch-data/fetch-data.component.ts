import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public examenes: Examen[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Examen[]>(baseUrl + 'api/examenes').subscribe(result => {
      this.examenes = result;
    }, error => console.error(error));
  }
}

interface Examen {
  name: string;
  tags: string;
}
