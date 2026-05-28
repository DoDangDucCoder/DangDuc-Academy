import { Component, Input } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { OverviewComponent } from './overview.component';

@Component({
  selector: 'app-breadcrumbs',
  template: '',
})
class BreadcrumbsStubComponent {
  @Input() title = '';
  @Input() breadcrumbItems: Array<unknown> = [];
}

@Component({
  selector: 'ngx-simplebar',
  template: '<ng-content></ng-content>',
})
class SimplebarStubComponent {}

describe('OverviewComponent', () => {
  let component: OverviewComponent;
  let fixture: ComponentFixture<OverviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [
        OverviewComponent,
        BreadcrumbsStubComponent,
        SimplebarStubComponent,
      ],
      imports: [RouterTestingModule],
    })
    .compileComponents();

    fixture = TestBed.createComponent(OverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
