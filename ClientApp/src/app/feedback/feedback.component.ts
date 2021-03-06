import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthorizeService } from 'src/api-authorization/authorize.service';

@Component({
  selector: 'app-feedback',
  templateUrl: './feedback.component.html',
  styleUrls: ['./feedback.component.css']
})
export class FeedbackComponent implements OnInit {
  model: any = {};
  _baseUrl: string;
  public _show: boolean;
  _sucSelected: boolean;
  _examSelected: boolean;
  _isEdit: boolean;
  _http: HttpClient;
  authorizeService: AuthorizeService;
  form: any;
  options = {
        headers: new HttpHeaders({
            'Content-Type': 'application/json'
        }),
        body: {}
    };
  // tslint:disable-next-line:max-line-length
  constructor(private router: Router, private route: ActivatedRoute, http: HttpClient, @Inject('BASE_URL') baseUrl: string, authorizeService: AuthorizeService) {
    this._baseUrl = baseUrl;
    this._http = http;
    this.authorizeService = authorizeService;
   }

  ngOnInit() {
  }
  sendData() {

        // tslint:disable-next-line:max-line-length
        const data = {feedback: this.model.feedback, apptId: this.route.snapshot.params.id};
        this._http.post(this._baseUrl + 'api/SetFeedback', data, this.options).subscribe(
          (response) => {
            alert('Gracias por sus comentarios');
            this.router.navigate([]).then((result) => {
              window.open('/', '_self');
            });
          },
          (error) => console.log(error)
        );
  }
}
