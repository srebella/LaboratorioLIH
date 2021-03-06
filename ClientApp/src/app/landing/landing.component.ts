import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
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
  logged: boolean;

  constructor(private authorizeService: AuthorizeService, @Inject('BASE_URL') baseUrl: string, http: HttpClient) {
    this._http = http;
    this._baseUrl = baseUrl;
   }

  ngOnInit() {
    this.authorizeService.isAuthenticated().subscribe(value => this.logged = value);
    if (this.logged) {
      this._http.get(this._baseUrl + 'api/GetUserById').subscribe(
        (response2) => {
          this.user = response2;
        }, error => console.error(error));
    }
  }

}
