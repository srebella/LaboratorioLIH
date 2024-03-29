import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbCalendar, NgbDateStruct, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { StringifyOptions } from 'querystring';
import { map } from 'rxjs/internal/operators/map';
import { AuthorizeService } from 'src/api-authorization/authorize.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  _baseUrl: string;
  public _show: boolean;
  _sucSelected: boolean;
  _examSelected: boolean;
  _isEdit: boolean;
  _http: HttpClient;
  today = this.calendar.getToday();
  public examenes: Examen[];
  public appointments: Appt[];
  public appointment: any = {};
  public sucursales: Sucursal[];
  public sucdata: Sucursal;
  public examdata: Examen;
  public allExams: Examen[];
  public allSucs: Sucursal[];
  public  authorizeService: AuthorizeService;
  apptid: string;
  model: any = {};
  form: any;
  options = {
        headers: new HttpHeaders({
            'Content-Type': 'application/json'
        }),
        body: {}
    };
  username: string;
  _isAdmin: boolean;
  // tslint:disable-next-line:max-line-length
  constructor(private modalService: NgbModal, private router: Router, private route: ActivatedRoute, private calendar: NgbCalendar, http: HttpClient, @Inject('BASE_URL') baseUrl: string, authorizeService: AuthorizeService) {
    this._baseUrl = baseUrl;
    this._http = http;
    this.authorizeService = authorizeService;
    this.model.hours = null;
    this.apptid = this.route.snapshot.queryParams['id'];
    if (this.apptid) {
      this.authorizeService.getUser().pipe(map(u => u && u.name)).subscribe(value => this.username = value);
        if (this.username === 'testuser654@mailinator.com') {
          // Admin access all turnos
          this._isEdit = true;
          this._isAdmin = true;
          this.getAppointmentsById();
        } else {
          // User accessing its turnos
          this._http.get<Appt[]>(this._baseUrl + 'api/GetAppointmentByUserId').subscribe(
            (response2) => {
              const userAppts = response2;
              // tslint:disable-next-line:triple-equals
              const hasAppt = userAppts.some(s => s.id == this.apptid);
              if (hasAppt) {
                this._isEdit = true;
                this.getAppointmentsById();
              } else {
                alert('El turno que desea ver no le pertenece');
                this.router.navigate([]).then((result) => {
                  window.open('/', '_self');
                });
              }
            }, error => console.error(error));
        }
    } else {
      this._http.get<Appt[]>(this._baseUrl + 'api/GetAppointmentByUserId').subscribe(
        (response2) => {
          const userAppts2 = response2;
          if (userAppts2.length > 0) {
            this._show = true;
            this.getAppointmentsByUserId();
          } else {
            this._show = false;
          }
        }, error => console.error(error));
    }
    http.get<Examen[]>(baseUrl + 'api/examenes').subscribe(result => {
      this.examenes = result;
      this.examenes.forEach(element => {
        element.requirements = element.requirements + element.protocols;
      });
    }, error => console.error(error));
    http.get<Sucursal[]>(baseUrl + 'api/GetSucursales').subscribe(result => {
      this.sucursales = result;
    }, error => console.error(error));
  }
  sendData() {
    // tslint:disable-next-line:max-line-length
    // if (confirm('Esta seguro que quiere confirmar turno?')) {
      if (!this._isEdit) {
        // tslint:disable-next-line:max-line-length
        const data = {userId: '', examId: this.model.examen, Date: this.model.date.year + '-' + this.model.date.month + '-' + this.model.date.day,
        Time: this.model.hours, SucursalId: this.model.sucursal };
        this._http.post<Examen>(this._baseUrl + 'api/SetAppointment', data, this.options).subscribe(
          (response) => {
            console.log(response);
            // this.getAppointmentsByUserId();
            if (this._isAdmin) {
              this.router.navigate([]).then((result) => {
                window.open('/admin', '_self');
              });
            } else {
              this.router.navigate([]).then((result) => {
                this._show = !this._show;
                window.open('/turnos', '_self');
              });
            }
          },
          (error) => console.log(error)
        );
      } else {
        // is Update
        // tslint:disable-next-line:max-line-length
        const data = {id: this.apptid, userId: '', examId: this.model.examen, Date: this.model.date.year + '-' + this.model.date.month + '-' + this.model.date.day,
        Time: String(this.model.hours), SucursalId: this.model.sucursal };
        this._http.post<Examen>(this._baseUrl + 'api/UpdateAppointment', data, this.options).subscribe(
          (response) => {
            console.log(response);
            // this.getAppointmentsByUserId();
            if (this._isAdmin) {
              this.router.navigate([]).then((result) => {
                window.open('/admin', '_self');
              });
            } else {
              this._show = !this._show;
              this.router.navigate([]).then((result) => {
                window.open('/turnos', '_self');
              });
            }
          },
          (error) => console.log(error)
        );
      }
    // }
  }
  searchSucursal(sucursalName) {
    this.sucdata =  this.sucursales.find(s => {
      return s.name === sucursalName.target.value;
   });
   this._sucSelected = true;
   console.log(this.sucdata);
  }
  searchExamen(examenName) {
    this.examdata =  this.examenes.find(s => {
      return s.name.includes(examenName.target.value) ? s : null;
   });
   if (this.examdata) {
    this._examSelected = true;
    console.log(this.examdata);
   }
  }
  verTurnos() {
    // this.getAppointmentsByUserId();
    if (this._isAdmin) {
      this.router.navigate([]).then((result) => {
        window.open('/admin', '_self');
      });
    } else {
      // this._show = !this._show;
      this.router.navigate([]).then((result) => {
        window.open('/turnos', '_self');
      });
    }
  }

    getAppointmentsByUserId() {
      this._http.get<Appt[]>(this._baseUrl + 'api/GetAppointmentByUserId').subscribe(
        (response2) => {
          this.appointments = response2;
        }, error => console.error(error));
    }
    getAppointmentsById() {
      this._http.get(this._baseUrl + 'api/GetAppointmentById?id=' + this.apptid).subscribe(
        (response) => {
          this.appointment = response;
          this.model.examen = this.appointment.examen.name;
          const d = new Date(this.appointment.date);
          this.model.date = d;
          this.model.date.year = d.getFullYear();
          this.model.date.month = d.getMonth() + 1;
          this.model.date.day = d.getDate();
          this.model.hours = d.getHours();
          this.model.sucursal = this.appointment.sucursal.name;
        }, error => console.error(error));
    }

  reload() {
    this._show = !this._show;
  }
  editarTurno(val) {
    this.router.navigate([]).then((result) => {
      window.open('/turnos?id=' + val, '_self');
    });
  }
  borrarTurno(id) {
    // if (confirm('Esta seguro que quiere borrar el turno?')) {
      this._http.get(this._baseUrl + 'api/DeleteApptById?id=' + id).subscribe(
        (response) => {
          if (response) {
            this.getAppointmentsByUserId();
          }
          return response;
        }, error => console.error(error));
    // }
  }
  openModal(content, videoId) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' }).result.then((result) => {
      // this.closeResult = `Closed with: ${result}`;
      if (result === 'yes') {
        this.borrarTurno(videoId);
      }
    }, (reason) => {
      // this.closeResult = `Dismissed ${this.getDismissReason(reason)}`;
    });
  }

  checkModel() {
    // tslint:disable-next-line:max-line-length
    return this.model.examen === undefined || this.model.sucursal === undefined || this.model.date === undefined || this.model.hours === undefined;
  }
  // private getDismissReason(reason: any): string {
  //   if (reason === ModalDismissReasons.ESC) {
  //     return 'by pressing ESC';
  //   } else if (reason === ModalDismissReasons.BACKDROP_CLICK) {
  //     return 'by clicking on a backdrop';
  //   } else {
  //     return `with: ${reason}`;
  //   }
  // }
}




interface Examen {
  id: string;
  userId: string;
  examId: string;
  name: string;
  date: string;
  time: string;
  sucursalId: string;
  requirements: string;
  protocols: string;
}
interface Sucursal {
  id: string;
  name: string;
  address: string;
}
interface Appt {
  id: string;
  userId: string;
  sucursal: string;
  examen: string;
  date: string;
}

