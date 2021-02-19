import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { NgbCalendar, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  _baseUrl: string;
  _http: HttpClient;
  today = this.calendar.getToday();
  public examenes: Examen[];
  public sucursales: Sucursal[];
  public allExams: Examen[];
  public allSucs: Sucursal[];
  model: any = {};
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
    http.get<Sucursal[]>(baseUrl + 'api/GetSucursales').subscribe(result => {
      this.sucursales = result;
    }, error => console.error(error));
  }

  sendData() {
    // const formData: any = new FormData();
    // formData.append('userId', '2');
    // formData.append('examId', this.form.get('examen').value); //   this.form.get('examanes').value);
    // formData.append('date', '1212-5-5');
    // formData.append('time', '9');
    // formData.append('sucursalId', this.form.get('sucursal').value);

    // tslint:disable-next-line:max-line-length
    const data = {userId: '', examId: this.model.examen, Date: this.model.date.year + '-' + this.model.date.month + '-' + this.model.date.day,
         Time: this.model.examen.hours, SucursalId: this.model.sucursal };
    this._http.post<Examen>(this._baseUrl + 'api/SetAppointment', data, this.options).subscribe(
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
interface Sucursal {
  Id: string;
  Name: string;
}

