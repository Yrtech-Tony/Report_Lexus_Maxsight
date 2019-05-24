/*
 * 搜索区域 js
 */
$(function () {
    var searchArea = $('#searchArea');
    var searchDealer = $('#searchDealer');
    var searchDate = $('#searchDate');
    var searchDealerGroup = $('#searchDealerGroup');
    var searchComm = $('#searchArea, #searchDealerGroup, #searchDealer');

    //显示/隐藏切换按钮
    searchComm.find('.switch').hover(function () {
        $(this).css('background', 'rgba(0,0,0,0.6)').find('a').show().css('top', ($(this).height() - 20) / 2);
    }, function () {
        $(this).css('background', '#fff').find('a').hide();
    });


    //切换到集团
    $('#toGroup').click(function () {
        //清空所有条件
        searchComm.find('input:checked').iCheck('uncheck');
        $('#inputBox').html('');
        searchDealer.find('ul').html('');

        //切换到集团
        searchDealerGroup.show();
        searchArea.hide();
    });

    //切换到区域
    $('#toArea').click(function () {
        //清空所有条件
        searchComm.find('input:checked').iCheck('uncheck');
        $('#inputBox').html('');
        searchDealer.find('ul').html('');

        //切换到集团
        searchArea.show();
        searchDealerGroup.hide();
    });

    //区域点击
    searchArea.find('input:radio').on('ifChecked', function (event) {
        getDealer($(this).val(), 1);
    });
    searchDealerGroup.find('input:radio').on('ifChecked', function (event) {
        getDealer($(this).val(), 2);
    });
    
    searchDate.find('li').click(function () {

    });
    
    //清空
    $('#btnClear').click(function () {
        $('.searchComm').find('input:checked').each(function () {
            $(this).iCheck('uncheck');
            $(this).removeAttr('checked').parent().attr('bChk', 2).removeClass("active");
        });
    });
});


/*
 * 经销店点击
 */
function dealerLiClk(obj) {
    updChkedInput();
}

/*
 * 根据区域、集团获得经销店
 * @param 			sValue  		指定区域、集团的值
 * @param 			nSort  			1-区域， 2-集团
 */
function getDealer(sValue, nSort) {
    if (sValue == '' || sValue == 'undefined') {
        alert('无法获取经销店，请检查！');
        return false;
    }

    $.ajax({
        url: '/Common/SearchShop',
        type: 'get',
        async: true,
        data: { nSort: nSort, sValue: sValue },
        success: function (data) {
            bindDealer(data);

            //heightHandle(searchDealer.find('p.ttl'));
        },
        error: function () {
            alert('获取经销店失败，请检查！');
        }
    });
};

function bindDealer(data) {
    var searchDealer = $('#searchDealer');
    var dealerUl = searchDealer.show().find('.searchComm').empty();
    $.each(data, function (index, item) {
        var chk = $("<input type='checkbox' name='nDealerID[]' value='" + item.ShopCode + "'>");
        var valueF = $('<font>').text(item.ShopName);
        var label = $("<label bChk='2' onclick='dealerLiClk(this)' class='dealer-li'></li>").append(chk).append(valueF);
        dealerUl.append(label);
    });

    $("input[type=checkbox]", dealerUl).iCheckParser();
}

/*
 * 获取所有已选条件，并追加到form
 */
function updChkedInput() {
    $('#inputBox').html('');

    $('.searchComm li input:checked').each(function () {
        $('#inputBox').append('<input type="hidden" name="' + $(this).attr('name') + '" value="' + $(this).val() + '" />');
    });
}

/*
 * 高度调整
 */
function heightHandle(objs) {
    objs.each(function () {
        var nH = $(this).siblings('ul').height() + 'px';
        $(this).css({ 'height': nH, 'lineHeight': nH });
    });
}

// 对Date的扩展，将 Date 转化为指定格式的String
// 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符， 
// 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字) 
// 例子： 
// (new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423 
// (new Date()).Format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18 
//调用： var time1 = new Date().Format("yyyy-MM-dd");var time2 = new Date().Format("yyyy-MM-dd HH:mm:ss");
Date.prototype.Format = function (fmt) { //author: meizz 
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "h+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}
 
//字符产格式
function ChangeDateFormat(d) {
    if (!d) return "";
    var date = new Date(parseInt(d.replace("/Date(", "").replace(")/", ""), 10));
    var month = padLeft(date.getMonth() + 1, 10);
    var currentDate = padLeft(date.getDate(), 10);
    var hour = padLeft(date.getHours(), 10);
    var minute = padLeft(date.getMinutes(), 10);
    return date.getFullYear() + "-" + month + "-" + currentDate;// + " " + hour + ":" + minute;
}
function padLeft(str, min) {
    if (str >= min)
        return str;
    else
        return "0" + str;
}

//字符串拼接
function stringFormat() {
    if (arguments.length == 0)
        return null;

    var str = arguments[0];
    for (var i = 1; i < arguments.length; i++) {
        var re = new RegExp('\\{' + (i - 1) + '\\}', 'gm');
        str = str.replace(re, arguments[i]);
    }
    return str;
}


jQuery.Hashtable = function () {
    this.items = new Array();
    this.itemsCount = 0;
    this.add = function (key, value) {
        if (!this.containsKey(key)) {
            this.items[key] = value;
            this.itemsCount++;
        }
        else {
            throw "key '" + key + "' allready exists.";
        }
    };

    this.get = function (key) {
        if (this.containsKey(key)) {
            return this.items[key];
        }
        else {
            return null;
        }
    };

    this.remove = function (key) {
        if (this.containsKey(key)) {
            delete this.items[key];
            this.itemsCount--;
        }
        else {
            throw "key '" + key + "' does not exists.";
        }
    };

    this.containsKey = function (key) {
        return typeof (this.items[key]) != "undefined";
    };

    this.containsValue = function containsValue(value) {
        for (var item in this.items) {
            if (this.items[item] == value) {
                return true;
            }
        }
        return false;
    };

    this.contains = function (keyOrValue) {
        return this.containsKey(keyOrValue) || this.containsValue(keyOrValue);
    };

    this.clear = function () {
        this.items = new Array();
        itemsCount = 0;
    };

    this.size = function () {
        return this.itemsCount;
    };

    this.isEmpty = function () {
        return this.size() == 0;
    };

    this.getKey = function () {
        aKeyName = new Array();
        var nIndex = 0;
        for (var sKeyName in this.items) {
            aKeyName[nIndex] = sKeyName;
            nIndex++;
        }

        return aKeyName;
    };
};


//根据ID获取指定元素当前值
//仅支持radio，checkbox，text/textare，如果不是这三种类型，则无法预期运行结果。
//注意：传入ID参数时，不要传入前面的#号。
function getValueByID(sID) {
    var obj = $("#" + sID);
    if (typeof (obj) != "undefined") {
        //type为radio的情形
        if (obj.attr("type") == "radio") {
            var sName = sID.substring(3);
            var sValue = $("input[name='" + sName + "']:checked").val();
            if (typeof (sValue) != "undefined") {
                return sValue;
            }
            else {
                return "";
            }
        }
        else {
            //type为checkbox的情形
            if (obj.attr("type") == "checkbox") {
                var sName = sID.substring(3) + "[]";
                var sValue = "";
                $("input[name='" + sName + "']:checked").each(function () {
                    sValue = sValue + $(this).val() + ",";
                });
                if (sValue.length > 1) {
                    sValue = sValue.substring(0, sValue.lastIndexOf(","));
                }
                return sValue;
            }
            else {
                //type为其他（预期为text/textarea）的情形
                if (typeof (obj.val()) != "undefined") {
                    return obj.val();
                }
                else {
                    return "";
                }
            }
        }
    }
    else {
        return "";
    }
}
//存储格式检查正则表达式的 Hashtable
var hChkFormat = new jQuery.Hashtable();

//修改检查格式正则表达式 Hashtable的函数。
function changeChkFormat(sElementID, nNullFlag, sRegex, sChkHint, sFormID) {
    var aTmp = new Array(2);
    aTmp[0] = nNullFlag;
    aTmp[1] = sRegex;
    aTmp[2] = sChkHint;
    aTmp[3] = sFormID;
    //var sKey = sFormID + "|" + sElementID;
    //hChkFormat.add(sKey, aTmp);
    hChkFormat.add(sElementID, aTmp);
    //alert(hChkFormat.size());
}

//检查格式函数
function chkFormat(sFormID) {
    var bReturn = true;
    var aKeyName = hChkFormat.getKey();
    for (nAKeyNameIdx = 0; nAKeyNameIdx < aKeyName.length; nAKeyNameIdx++) {
        var sElementID = aKeyName[nAKeyNameIdx];
        var aTmp = hChkFormat.get(sElementID);
        if (sFormID == aTmp[3]) {
            var bPerformChkFlag = true;
            var sElementValue = getValueByID(sElementID);
            if (aTmp[0] == "1" && sElementValue == "") {
                bPerformChkFlag = false;
            }

            if (bPerformChkFlag == true) {
                var regExp = new RegExp(aTmp[1]);
                if (regExp.test(sElementValue) == false) {
                    bReturn = false;
                    $("#" + sElementID + "_chkHint").text(aTmp[2]);
                }
                else {
                    $("#" + sElementID + "_chkHint").text("");
                }
            }
        }
    }

    return bReturn;
}

/**
 * 前四个变量为固定的，必须传送，从第5个开始，可选。第5个参数为提交时的状态提示语
 * 本函数适用于提交不带文件上传的Form
 */
//开始函数 submitForm
function submitForm(joForm, sURL, sSubmitValue, sHintEID) {
    var joHint = $('#' + sHintEID);
    var sSubmitHint = '正在提交，请稍候...';
    if (arguments.length > 4) {
        sSubmitHint = arguments[4];
    }
    joHint.text(sSubmitHint);
    joHint.show();

    var sData = joForm.serialize() + "&sSubmit=" + sSubmitValue;
    var insArtSubmitReq = $.ajax({ async: true, cache: false, dataType: "html", url: sURL, data: sData, type: "post" });
    insArtSubmitReq.done(function (msg) {
        var oMsg = null;
        try {
            oMsg = $.parseJSON(msg);
        }
        catch (e) {
            alert(e);
            alert(msg);
        }

        if (oMsg != null) {
            //如果不跳转，则给出提示
            if (oMsg.nAction != 2) {
                var sHint = oMsg.sHint;
                if (oMsg.sErrMsg != "") {
                    sHint += '[参考错误消息：' + oMsg.sErrMsg + ']';
                }
                joHint.text(sHint);
                setTimeout(function () { joHint.fadeOut(2000); }, 5000);
            }

            switch (oMsg.nAction) {
                case 1:
                    {
                        break;
                    }
                case 2:
                    {
                        var sRedirectURL = oMsg.sRedirectURL;
                        if (sRedirectURL == "") {
                            sRedirectURL = "/";
                        }
                        window.location.href = sRedirectURL;
                        break;
                    }
                case 3:
                    {
                        joForm[0].reset();
                        break;
                    }
                default:
                    {
                        window.location.href = "/";
                    }
            }
        }

    });
}
//结束函数 submitForm
//结束函数 submitForm
window.alert = function (msg, call) {
    if ($('#alterModal').length == 0) {
        $('body').append('<div id="alterModal" class="modal fade"><div class="modal-dialog modal-sm"><div class="modal-content"><div class="modal-header"><button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button><h4 class="modal-title">提示</h4></div><div class="modal-body"></div><div class="modal-footer"><button type="button" class="btn btn-default" data-dismiss="modal">确定</button></div></div></div></div>');
    }

    //
    $('#alterModal .modal-body').html('<p id="alert_msg">' + msg + '</p>');
    $('#alterModal').modal('show');
    $('#alterModal').on('hidden.bs.modal', function (e) {
        if (call) call();
    })
}

//结束函数 submitForm
window.confirm = function (msg, confirmCall, cancelCall) {
    if ($('#confirmModal').length > 0) {
        $('#confirmModal').remove();
    }
    $('body').append('<div id="confirmModal" class="modal fade">' +
           '<div class="modal-dialog modal-sm"><div class="modal-content">' +
           '<div class="modal-header"><button type="button" class="close" data-dismiss="modal" aria-label="Close">' +
           '<span aria-hidden="true">&times;</span></button><h4 class="modal-title">提示</h4></div><div class="modal-body"></div>' +
           '<div class="modal-footer"><button type="button" class="btn btn-default ok-btn" data-dismiss="modal">确定</button>' +
           '<button type="button" class="btn btn-primary cancel-btn" data-dismiss="modal">取消</button></div></div></div></div>');
    //
    $('#confirmModal .modal-body').html('<p id="confirm_msg">' + msg + '</p>');
    $('#confirmModal').modal('show');
    $('#confirmModal .ok-btn').click(function (e) {
        if (confirmCall) {
            confirmCall();
        }
    })

    $('#confirmModal .cancel-btn').click(function (e) {
        if (cancelCall) {
            cancelCall();
        }
    })
}

window.closealert = function () {
    $('#alterModal').modal('hide');
}