import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthorizeService } from 'src/api-authorization/authorize.service';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.css']
})
export class LandingComponent implements OnInit {
  public isAuthenticated: Observable<boolean>;
  public isNotAuthenticated: Observable<boolean>;
  private _http: any;
  private _baseUrl: string;
  user: any;
  logged: any;
  _router: any;

  constructor(private router: Router, private authorizeService: AuthorizeService, @Inject('BASE_URL') baseUrl: string, http: HttpClient) {
    this._http = http;
    this._baseUrl = baseUrl;
    this._router = router;
   }

  ngOnInit() {
      this._http.get(this._baseUrl + 'api/GetUserById').subscribe(
        (response) => {
          this.user = response;
          if (this.user.name) {
            this._http.get(this._baseUrl + 'api/CountAppointmentByUserId').subscribe(
              (response2) => {
                const userAppts = response2;
                if (userAppts) {
                  this.router.navigate([]).then((result) => {
                    window.open('/turnos', '_self');
                  });
                } else {
                  this.router.navigate([]).then((result) => {
                    window.open('/', '_self');
                  });
                }
              }, error => console.error(error));
          } else {
            this.user.name = '';
          }
        }, error => console.error(error));
  }
}
