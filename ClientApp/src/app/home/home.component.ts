import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { NgbCalendar, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  model: NgbDateStruct;
  _baseUrl: string;
  _http: HttpClient;
  today = this.calendar.getToday();
  public examenes: Examen[];
  form: any;

  constructor(private calendar: NgbCalendar, http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this._baseUrl = baseUrl;
    this._http = http;
    http.get<Examen[]>(baseUrl + 'examenes').subscribe(result => {
      this.examenes = result;
    }, error => console.error(error));
  }

  sendData() {
    const formData: any = new FormData();
    formData.append('userId', '3');
    formData.append('examId', this.form.get('examanes').value);
    formData.append('date', '1212-05-05');
    formData.append('time', '9');
    formData.append('sucursalId', '1');

    this._http.post('http://localhost:4000/examanes', formData).subscribe(
      (response) => console.log(response),
      (error) => console.log(error)
    );
  }
}
interface Examen {
  id: string;
  name: string;
}

