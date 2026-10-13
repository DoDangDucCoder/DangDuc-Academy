// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  baseUrl: 'http://localhost:5194/api/v1/',  // Thay đổi từ https://localhost:44319
  urlUploadFile: 'http://localhost:5148/api/v1/',  // Thay đổi từ https://localhost:44349
  clientIdGoogle: '506434420847-s7bcj6i2qqklv99ge93ubdhhl50e3son.apps.googleusercontent.com',
  facebookId: '',
  baseUrlHub: 'http://localhost:5194/',  // Thay đổi từ https://localhost:44319
  stripePublicKey: 'pk_test_51TiZJKJ7vs5Bw00NRojdusuKp0cCVkQ76jM8OnAsr5U77uBeCsBlFYdf0eZl3btvc4TF7wrfWdgcxldZ0nJFBWCO00EtdM6Hgq'
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/plugins/zone-error';  // Included with Angular CLI.
