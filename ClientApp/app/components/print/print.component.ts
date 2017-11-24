import { Component, ViewEncapsulation, Input, Output, OnChanges, ElementRef, EventEmitter } from '@angular/core';

@Component({
    selector: 'print-page',
    encapsulation: ViewEncapsulation.None,
    templateUrl: './print.component.html',
    styleUrls: ['./print.component.css']
})
export class PrintComponent implements OnChanges {

    @Input('section')
    section: string;
    @Output()
    sectionChange = new EventEmitter<any>();

    constructor(private ele: ElementRef) {
        if (this.section === undefined)
            this.section = '';
    }

    ngOnChanges(changes) {
        if (changes.section && changes.section.currentValue !== undefined && changes.section.currentValue !== '') {

        }
    }

    afterPrint() {
        console.log("after print");
        this.ele.nativeElement.children[0].innerHTML = '';
        this.sectionChange.emit('');
        this.section = '';

    }

    printDiv() {
        if (this.section && this.section != undefined && this.section != '') {
            var printContents = document.getElementById(this.section).innerHTML;
            var originalContents = document.body.innerHTML;

            if (window) {
                if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {
                    var popup = window.open('',
                        '_blank',
                        'width=740,height=600,scrollbars=no,menubar=no,toolbar=no,' +
                        'location=no,status=no,titlebar=no');

                    popup.window.focus();
                    popup.document.write('<!DOCTYPE html><html><head>  ' +
                        '<link rel="stylesheet" href="/print.css" media="screen,print">' +
                        '</head><body onload="window.print()">' +
                        printContents +
                        '</html>');
                    popup.onbeforeunload = function(event) {
                        popup.close();
                        //return '.\n';
                    };
                    popup.onabort = function(event) {
                        popup.document.close();
                        popup.close();
                    }
                } else {
                    var popup = window.open('', '_blank', 'width=800,height=600');
                    popup.document.open();
                    popup.document.write('<html><head>' +
                        '<link rel="stylesheet" href="print.css" media="screen,print">' +
                        '</head><body onload="window.print()">' +
                        printContents +
                        '</html>');
                    popup.document.close();
                }
            }
            return true;
        }
    }

}