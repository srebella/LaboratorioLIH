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
  _sucSelected: boolean;
  _http: HttpClient;
  today = this.calendar.getToday();
  public examenes: Examen[];
  public appointments: Appt[];
  public sucursales: Sucursal[];
  public sucdata: Sucursal;
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
              this.getAppointmentsById();
            },
            (error) => console.log(error)
          );
         }
  }
  onSortChange(e) {
  //   for (let d = 0, len = this.sucursales.length; d < len; d += 1) {
  //     if (this.sucursales[d].Name === e.target.value) {
  //       this.sucdata = this.sucursales[d];
  //       this._sucSelected = true;
  //     }
  // }
    this.sucdata =  this.sucursales.find(s => {
      return s.name === e.target.value;
   });
   this._sucSelected = true;
   console.log(this.sucdata);
  }
  verTurnos() {
    this._show = !this._show;
    this.getAppointmentsById();
  }

  getAppointmentsById() {
    this._http.get<Appt[]>(this._baseUrl + 'api/GetAppointmentByUserId').subscribe(
      (response2) => {
        this.appointments = response2;
      }, error => console.error(error));
  }

  reload() {
    this._show = !this._show;
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
  id: string;
  name: string;
  address: string;
}
interface Appt {
  id: string;
  userId: string;
  sucursalId: string;
  examenId: string;
  Date: string;
}

