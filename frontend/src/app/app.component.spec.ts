import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { AppComponent } from './app.component';
import { NotificationHubService } from './core/services/hub-notification.service';

describe('AppComponent', () => {
  let fixture: ComponentFixture<AppComponent>;
  const notificationHubService = jasmine.createSpyObj<NotificationHubService>(
    'NotificationHubService',
    ['startConnection']
  );

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AppComponent],
      imports: [RouterTestingModule],
      providers: [
        {
          provide: NotificationHubService,
          useValue: notificationHubService,
        },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
  });

  it('should create the app', () => {
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should start the notification hub on init', () => {
    expect(notificationHubService.startConnection).toHaveBeenCalled();
  });
});
