import { HttpClient, HttpHeaders } from '@angular/common/http';
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
  options = {
        headers: new HttpHeaders({
            'Content-Type': 'application/json'
        }),
        body: {}
    };
  constructor(private calendar: NgbCalendar, http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this._baseUrl = baseUrl;
    this._http = http;
    http.get<Examen[]>(baseUrl + 'api/examenes').subscribe(result => {
      this.examenes = result;
    }, error => console.error(error));
  }

  sendData() {
    const formData: any = new FormData();
    formData.append('userId', '3');
    formData.append('examId', '2'); //   this.form.get('examanes').value);
    formData.append('date', '1212-5-5');
    formData.append('time', '9');
    formData.append('sucursalId', '1');
    const data = {userId: '3', examId: '2', Date: '1212-5-5', Time: '9', SucursalId: '1'};
        this._http.post<Examen>(this._baseUrl + 'api/examenes/set', data, this.options).subscribe(
      (response) => console.log(response),
      (error) => console.log(error)
    );
  }
}

interface Examen {
  UserId: string;
  ExamId: string;
  Date: string;
  Time: string;
  SucursalId: string;
}

