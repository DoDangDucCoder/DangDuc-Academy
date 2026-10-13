import { Component, ViewChild } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { DecimalPipe } from '@angular/common';
import { DropzoneConfigInterface } from 'ngx-dropzone-wrapper';
import { Store } from '@ngrx/store';
import { addcourcelistData, deletecourcelistData, updatecourcelistData } from 'src/app/store/Learning-cources/cources.action';
import { PageChangedEvent } from 'ngx-bootstrap/pagination';
import { Options } from 'ngx-slider-v2';
import { Router } from '@angular/router';
import { CourseServices } from 'src/app/core/services/course.service';
import { Course } from 'src/app/models/models/course';
import { environment } from 'src/environments/environment';
import { ExportHelper } from 'src/app/core/helpers/export.helper';

@Component({
  selector: 'app-list-courses',
  templateUrl: './list-courses.component.html',
  styleUrls: ['./list-courses.component.scss'],
  providers: [DecimalPipe]
})

// List Component
export class ListCourseComponent {

  // bread crumb items
  breadCrumbItems!: Array<{}>;
  listForm!: UntypedFormGroup;
  submitted = false;
  term: any
  listData!: any;
  masterSelected!: boolean;
  files: File[] = [];
  courseList: any
  courses: Course[] = [];
  listCourseQuery: Course[] = [];
  endItem: any;


  // Price Slider
  pricevalue: number = 100;
  minVal: number = 100;
  maxVal: number = 1000;
  deleteId: any;
  priceoption: Options = {
    floor: 0,
    ceil: 2000,
    translate: (value: number): string => {
      return '$' + value;
    },
  };
  @ViewChild('addCourse', { static: false }) addCourse?: ModalDirective;
  @ViewChild('deleteRecordModal', { static: false }) deleteRecordModal?: ModalDirective;

  deleteID: any;
  isLoading = false;

  constructor(
    private formBuilder: UntypedFormBuilder,
    public store: Store,
    private router: Router,
    private readonly courseServices: CourseServices) {
  }

  ngOnInit(): void {
    this.breadCrumbItems = [
      { label: 'Course', active: true },
      { label: 'List of course', active: true }
    ];

    this.loadDataCourse();

    this.listForm = this.formBuilder.group({
      id: [''],
      img: [''],
      name: [''],
      category: ['', [Validators.required]],
      instructor: ['', [Validators.required]],
      lessons: ['', [Validators.required]],
      students: ['', [Validators.required]],
      duration: ['', [Validators.required]],
      fees: ['', [Validators.required]],
      status: ['', [Validators.required]]
    });
  }

  loadDataCourse() {
    this.isLoading = true;
    this.courseServices.getListAllCourse().subscribe((res) => {
      if (res.retCode == 0 && res.systemMessage == '') {
        this.courseList = res.data;
        this.listCourseQuery = res.data;
        this.courses = this.listCourseQuery.slice(0, 10);
        this.isLoading = false;
      } else {
        this.isLoading = false;
      }
    })
  }

  //  Filter Offcanvas Set
  openEnd() {
    document.getElementById('courseFilters')?.classList.add('show')
    document.querySelector('.backdrop3')?.classList.add('show')
  }

  closeoffcanvas() {
    document.getElementById('courseFilters')?.classList.remove('show')
    document.querySelector('.backdrop3')?.classList.remove('show')
  }

  valueChange(value: number, boundary: boolean): void {
    if (boundary) {
      this.minVal = value;
    } else {
      this.maxVal = value;
    }
  }

  // File Upload
  public dropzoneConfig: DropzoneConfigInterface = {
    url: `${environment.urlUploadFile}upload-video-to-server/file-document`,
    clickable: true,
    addRemoveLinks: true,
    previewsContainer: false,
  };

  uploadedFiles: any[] = [];

  imageURL: any;
  onUploadSuccess(event: any) {
    setTimeout(() => {
      this.uploadedFiles.push(event[0]);
      this.listForm.controls['img'].setValue(event[0].dataURL);
    }, 0);
  }

  removeFile(event: any) {
    this.uploadedFiles.splice(this.uploadedFiles.indexOf(event), 1);
  }

  // Sort Data
  direction: any = 'asc';
  onSort(column: any) {
    if (this.direction == 'asc') {
      this.direction = 'desc';
    } else {
      this.direction = 'asc';
    }
    const sortedArray = [...this.courses];
    sortedArray.sort((a, b) => {
      const res = this.compare(a[column], b[column]);
      return this.direction === 'asc' ? res : -res;
    });
    this.courses = sortedArray;
  }
  compare(v1: string | number, v2: string | number) {
    return v1 < v2 ? -1 : v1 > v2 ? 1 : 0;
  }

  saveProduct() {
    if (this.listForm.valid) {
      if (this.listForm.get('id')?.value) {
        const updatedData = this.listForm.value;
        this.store.dispatch(updatecourcelistData({ updatedData }));
      }
      else {
        const newData = this.listForm.value
        this.store.dispatch(addcourcelistData({ newData }));
      }
      setTimeout(() => {
        this.listForm.reset();
      }, 2000);
      this.uploadedFiles = [];
      this.addCourse?.hide()
    }
  }

  removeItem(id: any) {
    this.deleteID = id
    this.deleteRecordModal?.show()
  }

  deleteData(id: any) {
    this.deleteRecordModal?.hide();
    if (id) {
      this.store.dispatch(deletecourcelistData({ id: this.deleteID.toString() }));
    }
    this.deleteRecordModal?.hide();
    this.masterSelected = false
  }

  filterdata() {
    if (this.term) {
      this.courses = this.courseList.filter((el: any) => el.name.toLowerCase().includes(this.term.toLowerCase()))
    } else {
      this.courses = this.courseList.slice(0, 10)
    }
    this.updateNoResultDisplay();
  }

  updateNoResultDisplay() {
    const noResultElement = document.querySelector('.noresult') as HTMLElement;
    const paginationElement = document.getElementById('pagination-element') as HTMLElement
    if (this.term && this.courses.length === 0) {
      noResultElement.style.display = 'block';
      paginationElement.classList.add('d-none')
    } else {
      noResultElement.style.display = 'none';
      paginationElement.classList.remove('d-none')
    }
  }

  pageChanged(event: PageChangedEvent): void {
    const startItem = (event.page - 1) * event.itemsPerPage;
    this.endItem = event.page * event.itemsPerPage;
    this.courses = this.courseList.slice(startItem, this.endItem);
  }

  addNewCourse() {
    this.router.navigate(['/learning/courses/create-course']);
  }

  editCourse(id: number) {
    this.router.navigate([`/learning/courses/edit-course/${id}`]);
  }

  goToOverview(id: number) {
    this.router.navigate([`/learning/courses/course-overview/${id}`]);
  }

  exportCourses() {
    if (!this.courseList || this.courseList.length === 0) return;
    const exportData = this.courseList.map((c: any) => ({
      ID: c.id,
      Title: c.title || '',
      Category: c.categoryName || '',
      Price: c.priceOfCourse || 0,
      DiscountPrice: c.priceOfDiscount || 0,
      IsFree: c.isFree ? 'Yes' : 'No',
      Level: c.levelCourse || '',
      Status: c.status
    }));
    ExportHelper.exportToCSV(exportData, 'courses');
  }

}
