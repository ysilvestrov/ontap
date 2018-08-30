import {IPub, ICity, Pub, IServe} from "../../models/ontap.models";

import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: "sortByTap"
})
export class SortByTap implements PipeTransform {
    isNumber(value: string | number): boolean {
        return !isNaN(Number(value.toString()));
    }

    transform(array: Array<IServe>, args: string): Array<IServe> {
        array.sort((a: IServe, b: IServe) => {
            if (this.isNumber(a.tap) && this.isNumber(b.tap)) {
                if (Number(a.tap) < Number(b.tap)) {
                    return -1;
                } else if (Number(a.tap) > Number(b.tap)) {
                    return 1;
                } else {
                    return 0;
                }
            } else if (a.tap < b.tap) {
                return -1;
            } else if (a.tap > b.tap) {
                return 1;
            } else {
                return 0;
            }
        });
        return array;
    }
}