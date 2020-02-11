import { IMyOptions } from 'ng-uikit-pro-standard';

export class Utils {

    static myDatePickerOptions: IMyOptions = {
        // Strings and translations
        dayLabels: {su: 'Dim', mo: 'Lun', tu: 'Mar', we: 'Mer', th: 'Jeu', fr: 'Ven', sa: 'Sam'},
        dayLabelsFull: {su: 'Dimanche', mo: 'Lundi', tu: 'Mardi', we: 'Mercredi', th: 'Jeudi', fr: 'Vendredi', sa:
        'Samedi'},
        monthLabels: { 1: 'Jan', 2: 'Fev', 3: 'Mar', 4: 'Avr', 5: 'Mai', 6: 'Juin', 7: 'Juil', 8: 'Août', 9: 'Sept', 10:
        'Oct', 11: 'Nov', 12: 'Dec' },
        monthLabelsFull: { 1: 'Janvier', 2: 'Fevrier', 3: 'Mars', 4: 'Avril', 5: 'Mai', 6: 'Juin', 7: 'Juillet', 8:
        'Août', 9: 'Septembre', 10: 'Octobre', 11: 'Novembre', 12: 'Decembre' },

        // Buttons
        todayBtnTxt: 'Auj',
        clearBtnTxt: 'effacer',
        closeBtnTxt: 'fermer',

        // format
        dateFormat: 'dd/mm/yyyy',

        // auto close
        closeAfterSelect: true,

        // First day of the week
        // firstDayOfWeek: 'mo',

        // // // Disable dates
        // disableUntil: {year: 2018, month: 10, day: 1},
        // disableSince: {year: 2018, month: 10, day: 31},
        // disableDays: [{year: 2018, month: 10, day: 3}],
        // disableDateRanges: [{begin: {year: 2018, month: 10, day: 5}, end: {year: 2018, month: 10, day: 7}}],
        // disableWeekends: false,

        // // Enable dates (when disabled)

        // // Year limits
        // minYear: 1000,
        // maxYear: 9999,

        // // Show Today button
        // showTodayBtn: true,

        // // Show Clear date button
        // showClearDateBtn: true,

        // markCurrentDay: true,
        // markDates: [{dates: [{year: 2018, month: 10, day: 20}], color: '#303030'}],
        // markWeekends: undefined,
        // disableHeaderButtons: false,
        // showWeekNumbers: false,
        // height: '100px',
        // width: '50%',
        // selectionTxtFontSize: '15px'
      };

    static isMobile() {
        return window && window.matchMedia('(max-width: 767px)').matches;
    }
    static ngbDateToDate(ngbDate: { month, day, year }) {
        if (!ngbDate) {
            return null;
        }
        return new Date(`${ngbDate.month}/${ngbDate.day}/${ngbDate.year}`);
    }
    static dateToNgbDate(date: Date) {
        if (!date) {
            return null;
        }
        date = new Date(date);
        return { month: date.getMonth() + 1, day: date.getDate(), year: date.getFullYear() };
    }
    static scrollToTop(selector: string) {
        if (document) {
            const element = <HTMLElement>document.querySelector(selector);
            element.scrollTop = 0;
        }
    }
    static genId() {
        let text = '';
        const possible = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
        for (let i = 0; i < 5; i++) {
            text += possible.charAt(Math.floor(Math.random() * possible.length));
        }
        return text;
    }

    // date : dd/mm/yyyy where char = /
    static inputDateDDMMYY(date: string, char: string) {
        const dt = date.split(char);
        // console.log('table dt:' + dt);
        return new Date(Number(dt[2]), Number(dt[1]) - 1, Number(dt[0]));
    }

    static smoothScrollToTop() {
        const scrollToTop = window.setInterval(() => {
          const pos = window.pageYOffset;
          if (pos > 0) {
              window.scrollTo(0, pos - 10); // how far to scroll on each step
          } else {
              window.clearInterval(scrollToTop);
          }
        }, 10);
    }

}
