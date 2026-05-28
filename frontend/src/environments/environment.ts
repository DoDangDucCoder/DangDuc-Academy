// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  baseUrl: 'http://localhost:5194/api/v1/',  // Thay đổi từ https://localhost:44319
  urlUploadFile: 'http://localhost:5148/api/v1/',  // Thay đổi từ https://localhost:44349
  clientIdGoogle: '458553878162-a7m0q6jreoirc837m3dp06mtmv5t5cu7.apps.googleusercontent.com',
  facebookId: '727016849494944',
  baseUrlHub: 'http://localhost:5194/',  // Thay đổi từ https://localhost:44319
  stripePublicKey: 'pk_test_51N6HwWLcCqr269TdVni7gwhS4QTPyR4UilZqxouP3KjAIRuPx66on5f97oraQ9zY3wFrzfuJKgc3s3eLzAaihKtb00K8D1MLh8'
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/plugins/zone-error';  // Included with Angular CLI.
