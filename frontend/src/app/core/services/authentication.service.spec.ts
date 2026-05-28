import { of } from 'rxjs';
import { AuthenticationServices } from './authentication.service';
import { TransferHttp } from '../transfer-http/transfer-http';

describe('AuthenticationServices', () => {
  let service: AuthenticationServices;
  let transferHttpSpy: jasmine.SpyObj<TransferHttp>;

  beforeEach(() => {
    transferHttpSpy = jasmine.createSpyObj<TransferHttp>('TransferHttp', ['get', 'post']);
    transferHttpSpy.get.and.returnValue(of({}));
    transferHttpSpy.post.and.returnValue(of({}));

    service = new AuthenticationServices(transferHttpSpy);
  });

  it('should call register endpoint with body payload', () => {
    service.registerUser('Test User', '0900/000 000', 'test+demo@example.com', 'se/cret?1').subscribe();

    expect(transferHttpSpy.post).toHaveBeenCalledWith('authentication/register', {
      name: 'Test User',
      phone: '0900/000 000',
      email: 'test+demo@example.com',
      password: 'se/cret?1'
    });
  });
});
