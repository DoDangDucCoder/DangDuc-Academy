import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GlobalComponent } from "../../global-component";


@Injectable({
  providedIn: 'root'
})
export class restApiService {
  constructor(private http: HttpClient) { }

  private getAuthHeaders(): HttpHeaders {
    const token = sessionStorage.getItem('auth-token');

    return new HttpHeaders(
      token ? { Authorization: `Bearer ${token}` } : {}
    );
  }

  private getJsonHttpOptions(): { headers: HttpHeaders } {
    return {
      headers: this.getAuthHeaders().set('Content-Type', 'application/json'),
    };
  }

  private getTextHttpOptions(): { headers: HttpHeaders; responseType: 'text' } {
    return {
      headers: this.getAuthHeaders(),
      responseType: 'text',
    };
  }

  /**
  * Product Rest Api
  */
  // Get
  getData(): Observable<any> {
    return this.http.get(GlobalComponent.API_URL + GlobalComponent.product, this.getTextHttpOptions());
  }

  // Delete 
  deleteData(id:any): Observable<any> {
    return this.http.delete(GlobalComponent.API_URL + GlobalComponent.productDelete + id, { responseType: 'text' });
  }

  /**
  * Order Rest Api
  */
  // Get
  getOrderData(): Observable<any> {
    return this.http.get(GlobalComponent.API_URL + GlobalComponent.order, this.getTextHttpOptions());
  }

  // POST
  postOrderData(employee: any): Observable<any> {
    return this.http.post(GlobalComponent.API_URL + GlobalComponent.order, JSON.stringify(employee), this.getJsonHttpOptions());
  }

  // Single
  getSingleOrderData(id:any): Observable<any> {
    return this.http.get(GlobalComponent.API_URL + GlobalComponent.orderId+id, this.getTextHttpOptions());
  }

  //Order Patch
  patchOrderData(employee: any): Observable<any> {   
    return this.http.patch(GlobalComponent.API_URL + GlobalComponent.orderId+employee.ids, JSON.stringify(employee), this.getJsonHttpOptions());
  }

  // Order Delete
  deleteOrder(id:any): Observable<any> {
    return this.http.delete(GlobalComponent.API_URL + GlobalComponent.orderId + id, this.getTextHttpOptions());
  }

  
  /**
  * Customers Rest Api
  */
  // Get
  getCustomerData(): Observable<any> {
    return this.http.get(GlobalComponent.API_URL + GlobalComponent.customer, this.getTextHttpOptions());
  }

  // POST
  postCustomerData(customers: any): Observable<any> {        
    return this.http.post(GlobalComponent.API_URL + GlobalComponent.customer, JSON.stringify(customers), this.getJsonHttpOptions());
  }

  // Single
  getSingleCustomerData(id:any): Observable<any> {
    return this.http.get(GlobalComponent.API_URL + 'apps/customer/' + id, this.getTextHttpOptions());
  }

  // Patch
  patchCustomerData(customers: any): Observable<any> {    
    return this.http.patch(GlobalComponent.API_URL + 'apps/customer/' + customers.ids, JSON.stringify(customers), this.getJsonHttpOptions());
  }

  // Delete
  deleteCustomer(id:any): Observable<any> {
    return this.http.delete(GlobalComponent.API_URL + 'apps/customer/' + id, this.getTextHttpOptions());
  }

  /**
   * Task List Rest Api
   */
  // Get
  getTaskData(): Observable<any> {
    return this.http.get(GlobalComponent.API_URL + 'apps/task', this.getTextHttpOptions());
  }

  // POST
  postTaskData(task: any): Observable<any> {        
    return this.http.post(GlobalComponent.API_URL + 'apps/task', JSON.stringify(task), this.getJsonHttpOptions());
  }

  // Single
  getSingleTaskData(id:any): Observable<any> {
    return this.http.get(GlobalComponent.API_URL + 'apps/task/' + id, this.getTextHttpOptions());
  }

  // Patch
  patchTaskData(tasks: any): Observable<any> {    
    return this.http.patch(GlobalComponent.API_URL + 'apps/task/' + tasks.ids, JSON.stringify(tasks), this.getJsonHttpOptions());
  }

  // Delete
  deleteTask(id:any): Observable<any> {
    return this.http.delete(GlobalComponent.API_URL + 'apps/task/' + id, this.getTextHttpOptions());
  }

  /**
   * CRM Contect Rest Api
   */
  // Get
  getContactData(): Observable<any> {
    return this.http.get(GlobalComponent.API_URL + 'apps/contact', this.getTextHttpOptions());
  }

  // POST
  postContactData(contact: any): Observable<any> {            
    return this.http.post(GlobalComponent.API_URL + 'apps/contact', JSON.stringify(contact), this.getJsonHttpOptions());
  }

  // Single
  getSingleContactData(id:any): Observable<any> {
    return this.http.get(GlobalComponent.API_URL + 'apps/contact/' + id, this.getTextHttpOptions());
  }

  // Patch
  patchContactData(contacts: any): Observable<any> {    
    return this.http.patch(GlobalComponent.API_URL + 'apps/contact/' + contacts.ids, JSON.stringify(contacts), this.getJsonHttpOptions());
  }

   // Delete
   deleteContact(id:any): Observable<any> {
    return this.http.delete(GlobalComponent.API_URL + 'apps/contact/' + id, this.getTextHttpOptions());
  }

  /**
   * CRM Company Rest Api
   */
  // Get
  getCompanyData(): Observable<any> {
    return this.http.get(GlobalComponent.API_URL + 'apps/company', this.getTextHttpOptions());
  }

  // POST
  postCompanyData(company: any): Observable<any> {            
    return this.http.post(GlobalComponent.API_URL + 'apps/company', JSON.stringify(company), this.getJsonHttpOptions());
  }

  // Single
  getSingleCompanyData(id:any): Observable<any> {
    return this.http.get(GlobalComponent.API_URL + 'apps/company/' + id, this.getTextHttpOptions());
  }

  // Patch
  patchCompanyData(company: any): Observable<any> {    
    return this.http.patch(GlobalComponent.API_URL + 'apps/company/' + company.ids, JSON.stringify(company), this.getJsonHttpOptions());
  }

  // Delete
  deleteCompany(id:any): Observable<any> {
    return this.http.delete(GlobalComponent.API_URL + 'apps/company/' + id, this.getTextHttpOptions());
  }

  /* CRM Company Rest Api
  */
  // Get
  getLeadData(): Observable<any> {
    return this.http.get(GlobalComponent.API_URL + 'apps/lead', this.getTextHttpOptions());
  }

   // POST
   postLeadData(company: any): Observable<any> {            
    return this.http.post(GlobalComponent.API_URL + 'apps/lead', JSON.stringify(company), this.getJsonHttpOptions());
  }

  // Single
  getSingLeadData(id:any): Observable<any> {
    return this.http.get(GlobalComponent.API_URL + 'apps/lead/' + id, this.getTextHttpOptions());
  }

  // Patch
  patchLeadData(company: any): Observable<any> {    
    return this.http.patch(GlobalComponent.API_URL + 'apps/lead/' + company.ids, JSON.stringify(company), this.getJsonHttpOptions());
  }

  // Delete
  deletelead(id:any): Observable<any> {
    return this.http.delete(GlobalComponent.API_URL + 'apps/lead/' + id, this.getTextHttpOptions());
  }


  /* Support Ticket Rest Api
  */
  // Get
  getTicketData(): Observable<any> {
    return this.http.get(GlobalComponent.API_URL + 'apps/ticket', this.getTextHttpOptions());
  }

  // POST
  postTicketData(ticket: any): Observable<any> {            
    return this.http.post(GlobalComponent.API_URL + 'apps/ticket', JSON.stringify(ticket), this.getJsonHttpOptions());
  }

  // Single
  getSingTicketData(id:any): Observable<any> {
    return this.http.get(GlobalComponent.API_URL + 'apps/ticket/' + id, this.getTextHttpOptions());
  }

   // Patch
   patchTicketData(ticket: any): Observable<any> {    
    return this.http.patch(GlobalComponent.API_URL + 'apps/ticket/' + ticket.ids, JSON.stringify(ticket), this.getJsonHttpOptions());
  }

  // Delete
  deleteTicket(id:any): Observable<any> {
    return this.http.delete(GlobalComponent.API_URL + 'apps/ticket/' + id, this.getTextHttpOptions());
  }

  /**
  * Support Ticket Rest Api
  */
  // Get
  getInvoiceData(): Observable<any> {
    return this.http.get(GlobalComponent.API_URL + 'apps/invoice', this.getTextHttpOptions());
  }

   // Delete
   deleteInvoice(id:any): Observable<any> {
    return this.http.delete(GlobalComponent.API_URL + 'apps/invoice/' + id, this.getTextHttpOptions());
  }

  /**
  * Todo Rest Api
  */
  // Get
  getTodoData(): Observable<any> {
    return this.http.get(GlobalComponent.API_URL + 'apps/todo/', this.getTextHttpOptions());
  }

  // POST
  postTodoData(employee: any): Observable<any> {
    return this.http.post(GlobalComponent.API_URL + 'apps/todo/', JSON.stringify(employee), this.getJsonHttpOptions());
  }

  // Single
  getSingleTodoData(id:any): Observable<any> {
    return this.http.get(GlobalComponent.API_URL + 'apps/todo/'+id, this.getTextHttpOptions());
  }

  //Patch
  patchTodoData(employee: any): Observable<any> {   
    return this.http.patch(GlobalComponent.API_URL + 'apps/todo/'+employee.ids, JSON.stringify(employee), this.getJsonHttpOptions());
  }

  /**
  * Calender Rest Api
  */
  // Get
  getCalendarData(): Observable<any> {
    return this.http.get(GlobalComponent.API_URL + 'apps/calendar/', this.getTextHttpOptions());
  }


}
