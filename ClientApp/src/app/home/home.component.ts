import { Component } from '@angular/core';
import { NgbCalendar, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  model: NgbDateStruct;
  today = this.calendar.getToday();
  constructor(private calendar: NgbCalendar) { }
}
