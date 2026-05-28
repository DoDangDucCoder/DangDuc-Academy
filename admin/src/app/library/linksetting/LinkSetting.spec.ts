import { LinkSettings } from './LinkSetting';

describe('LinkSettings', () => {
  it('should resolve static authentication links without path secrets', () => {
    const link = LinkSettings.GetResLinkSetting(
      'Authentication',
      'RegisterUser'
    );

    expect(link).toBe('authentication/register');
  });
});
