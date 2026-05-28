import { Component, OnInit } from '@angular/core';
import { FormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { MessengerServices } from 'src/app/core/services/messenger.service';
import { StudentServices } from 'src/app/core/services/student.service';
import { LocalStorageConfig } from 'src/app/library/clientconfig/localstorageconfig';
import { StudentUser } from 'src/app/models/models/student-user';

@Component({
  selector: 'app-account-profile-details',
  templateUrl: './account-profile-details.component.html',
  styleUrls: ['./account-profile-details.component.scss']
})
export class AccountProfileDetailsComponent implements OnInit {

  student: StudentUser | null = null;
  isLoading: boolean = false;
  informationOfUser!: UntypedFormGroup;
  generals = [
    { id: 0, name: 'Nam' },
    { id: 1, name: 'Nữ' },
  ];
  constructor(
    private readonly studentServices: StudentServices,
    private readonly messengerServices: MessengerServices,
    private formBuilder: FormBuilder,
  ) { }

  ngOnInit() {
    this.informationOfUser = this.formBuilder.group({
      id: [],
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: [''],
      phone: ['', [Validators.required]],
      general: [''],
    });
    this.getDataOfStudent();
  }

  getDataOfStudent() {
    const getUserLocal = LocalStorageConfig.GetUser();
    if (!getUserLocal) {
      this.student = null;
      this.isLoading = false;
      this.messengerServices.warringBookMarkCourse('Vui lòng đăng nhập lại để xem thông tin tài khoản!');
      return;
    }

    this.isLoading = true;
    this.studentServices.getStudent(getUserLocal.userId).subscribe(res => {
      if (res.retCode == 0 && res.systemMessage == '') {
        this.student = res.data ?? null;
        this.isLoading = false;
        if (this.student) {
          this.informationOfUser.patchValue(this.student);
        }
      } else {
        this.isLoading = false;
        this.messengerServices.errorWithIssue();
      }
    })
  }

  updateStudent() {
    if (!this.student) {
      this.messengerServices.warringBookMarkCourse('Không tìm thấy thông tin tài khoản để cập nhật.');
      return;
    }

    if (this.informationOfUser.valid) {
      this.isLoading = true;
      this.student.name = this.informationOfUser.value.name;
      this.student.email = this.informationOfUser.value.email;
      this.student.password = this.informationOfUser.value.password;
      this.student.phone = this.informationOfUser.value.phone;
      this.student.general = this.informationOfUser.value.general;
      this.studentServices.updateStudent(this.student).subscribe(res => {
        if (res.retCode == 0 && res.systemMessage == '') {
          this.isLoading = false;
          this.messengerServices.successes('Cập nhật thông tin người dùng thành công');
          this.getDataOfStudent();
        } else {
          this.isLoading = false;
          this.messengerServices.errorWithIssue();
        }
      })
    } else {
      this.messengerServices.warringWithIssue();
    }

  }

}
