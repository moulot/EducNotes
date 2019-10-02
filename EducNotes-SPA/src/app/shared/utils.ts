export class Utils {
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
