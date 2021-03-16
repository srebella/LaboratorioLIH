import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbCalendar, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthorizeService } from 'src/api-authorization/authorize.service';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  private _http: any;
  private _baseUrl: string;
  isEdit: boolean;
  appointments: any;
  user = {
    role: 'false'
  };
  authorizeService: any;
  username: any;
  // tslint:disable-next-line:max-line-length
  constructor(private modalService: NgbModal, private router: Router, private route: ActivatedRoute, private calendar: NgbCalendar, http: HttpClient, @Inject('BASE_URL') baseUrl: string, authorizeService: AuthorizeService) {
    this._baseUrl = baseUrl;
    this._http = http;
    this.authorizeService = authorizeService;
  }

  ngOnInit() {
    this.authorizeService.getUser().subscribe(value => this.username = value);
      if (this.username.name === 'testuser654@mailinator.com') {
        this.user.role = 'admin';
        this.getAppointmentsByUserId();
        this.isEdit = false;
      } else {
        alert('Usuario no válido para acceder al panel administrador');
        this.router.navigate([]).then((result) => {
          window.open('/', '_self');
        });
      }
  }
  getUsername() {
    return JSON.parse(localStorage.getItem('currentUser')).email;
  }
  getAppointmentsByUserId() {
    this._http.get(this._baseUrl + 'api/GetAppointments').subscribe(
      (response2) => {
        this.appointments = response2;
      }, error => console.error(error));
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
  //  }
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
}
interface Appt {
  id: string;
  userId: string;
  sucursal: string;
  examen: string;
  date: string;
}
