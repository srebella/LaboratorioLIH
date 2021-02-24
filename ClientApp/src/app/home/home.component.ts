import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { NgbCalendar, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  _baseUrl: string;
  public _show: boolean;
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
  constructor(private router: Router, private calendar: NgbCalendar, http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this._baseUrl = baseUrl;
    this._http = http;
    this.model.hours = null;
    http.get<Examen[]>(baseUrl + 'api/examenes').subscribe(result => {
      this.examenes = result;
    }, error => console.error(error));
    http.get<Sucursal[]>(baseUrl + 'api/GetSucursales').subscribe(result => {
      this.sucursales = result;
    }, error => console.error(error));
  }

  sendData() {
    // tslint:disable-next-line:max-line-length
    if (confirm('Esta seguro que quiere confirmar turno?')) {
        // tslint:disable-next-line:max-line-length
        const data = {userId: '', examId: this.model.examen, Date: this.model.date.year + '-0' + this.model.date.month + '-' + this.model.date.day,
         Time: this.model.hours, SucursalId: this.model.sucursal };
          this._http.post<Examen>(this._baseUrl + 'api/SetAppointment', data, this.options).subscribe(
            (response) => {
              console.log(response);
              this._show = !this._show;
            }
              ,
            (error) => console.log(error)
          );
         }
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

