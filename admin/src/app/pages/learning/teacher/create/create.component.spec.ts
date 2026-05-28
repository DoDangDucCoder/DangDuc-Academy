import { Component, Directive, Input } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';

import { CreateComponent } from './create.component';

@Component({
  selector: 'app-breadcrumbs',
  template: '',
})
class BreadcrumbsStubComponent {
  @Input() title = '';
  @Input() breadcrumbItems: Array<unknown> = [];
}

@Component({
  selector: 'ng-select',
  template: '',
})
class NgSelectStubComponent {
  @Input() items: Array<unknown> = [];
  @Input() multiple = false;
  @Input() bindLabel = '';
  @Input() bindValue = '';
}

@Directive({
  selector: '[mwlFlatpickr]',
})
class FlatpickrStubDirective {
  @Input() altInput = false;
  @Input() convertModelValue = false;
  @Input() altFormat = '';
  @Input() dateFormat = '';
}

describe('CreateComponent', () => {
  let component: CreateComponent;
  let fixture: ComponentFixture<CreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [
        CreateComponent,
        BreadcrumbsStubComponent,
        NgSelectStubComponent,
        FlatpickrStubDirective,
      ],
      imports: [ReactiveFormsModule],
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
