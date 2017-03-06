import { Component } from '@angular/core';
import { DialogComponent, DialogService } from "ng2-bootstrap-modal";
import { Options } from "../../modules/appComponent";

export interface ISelectModel {
    title: string;
    message: string;
    options: Options[];
}
@Component({
    selector: 'confirm',
    templateUrl: './selector.component.html'
})
export class SelectorComponent extends DialogComponent<ISelectModel, any> implements ISelectModel {
    title: string;
    message: string;
    options: Options[];
    selected: any;

    constructor(dialogService: DialogService) {
        super(dialogService);
    }
    confirm() {
        // we set dialog result as true on click on confirm button, 
        // then we can get dialog result from caller code 
        this.result = this.selected;
        this.close();
    }
}