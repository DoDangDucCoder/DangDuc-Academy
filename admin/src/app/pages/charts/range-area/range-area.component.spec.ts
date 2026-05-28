import { Component, Input } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RangeAreaComponent } from './range-area.component';

@Component({
  selector: 'app-breadcrumbs',
  template: '',
})
class BreadcrumbsStubComponent {
  @Input() title = '';
  @Input() breadcrumbItems: Array<unknown> = [];
}

@Component({
  selector: 'apx-chart',
  template: '',
})
class ApexChartStubComponent {
  @Input() series: unknown;
  @Input() chart: unknown;
  @Input() plotOptions: unknown;
  @Input() labels: unknown;
  @Input() colors: unknown;
  @Input() dataLabels: unknown;
  @Input() title: unknown;
  @Input() stroke: unknown;
  @Input() fill: unknown;
  @Input() forecastDataPoints: unknown;
}

describe('RangeAreaComponent', () => {
  let component: RangeAreaComponent;
  let fixture: ComponentFixture<RangeAreaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [
        RangeAreaComponent,
        BreadcrumbsStubComponent,
        ApexChartStubComponent,
      ],
    })
    .compileComponents();

    fixture = TestBed.createComponent(RangeAreaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
