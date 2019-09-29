import { FormArray, FormControl, FormGroup, ValidationErrors } from '@angular/forms';

export class CustomValidators {

  static maxGradeValidator(form: FormGroup): ValidationErrors {
    const isGradeCtrl = form.get('isGraded');
    const evalMaxGradeCtrl = form.get('evalMaxGrade');

    if (isGradeCtrl != null && evalMaxGradeCtrl != null) {

      let error = null;

      if (isGradeCtrl.value === 'true' && evalMaxGradeCtrl.value === 'false') {
        error = 'l\'évaluation est notée. la note maximale est obligatoire';
      }

      const message = {
        'isGradedMaxGrade': {
          'message': error
        }
      };

      return error ? message : null;
    }
  }

  // static countryCity(form: FormGroup): ValidationErrors {
  //   const countryControl = form.get('country');
  //   const cityControl = form.get('city');

  //   if (countryControl != null && cityControl != null) {
  //     const country = countryControl.value;
  //     const city = cityControl.value;
  //     let error = null;

  //     if (country === 'France' && city !== 'Paris') {
  //       error = 'If the country is France, the city must be Paris';
  //     }

  //     const message = {
  //       'countryCity': {
  //         'message': error
  //       }
  //     };

  //     return error ? message : null;
  //   }
  // }

  // static uniqueName(c: FormControl): Promise<ValidationErrors> {
  //   const message = {
  //     'uniqueName': {
  //       'message': 'The name is not unique'
  //     }
  //   };

  //   return new Promise(resolve => {
  //     setTimeout(() => {
  //       resolve(c.value === 'Existing' ? message : null);
  //     }, 1000);
  //   });
  // }

  // static telephoneNumber(c: FormControl): ValidationErrors {
  //   const isValidPhoneNumber = /^\d{3,3}-\d{3,3}-\d{3,3}$/.test(c.value);
  //   const message = {
  //     'telephoneNumber': {
  //       'message': 'The phone number must be valid (XXX-XXX-XXX, where X is a digit)'
  //     }
  //   };
  //   return isValidPhoneNumber ? null : message;
  // }

  // static telephoneNumbers(form: FormGroup): ValidationErrors {

  //   const message = {
  //     'telephoneNumbers': {
  //       'message': 'At least one telephone number must be entered'
  //     }
  //   };

  //   const phoneNumbers = form.controls;
  //   const hasPhoneNumbers = phoneNumbers && Object.keys(phoneNumbers).length > 0;

  //   return hasPhoneNumbers ? null : message;
  // }
}
