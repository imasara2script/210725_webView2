	WPF = function(){
		var arrCallBack = []
		return {
			send:(str, callBack)=>{
				const pM = str => window.chrome.webview.postMessage(str+'') // postMessageで送れるのは文字列だけなので''を付加して文字列化している。
				
				if(!callBack){return pM(str)}
				
				let i = 0
				while(arrCallBack[i]){ i++ }
				arrCallBack[i] = callBack
				
				pM(str + '=' + i)
			},
			callBack:(ind, value)=>{
				arrCallBack[ind](value)
				arrCallBack[ind] = 0
			},
			sendPromise:function(str){
				const this_ = this
				return new Promise((resolve, reject)=>{
					this_.send(str, (v)=> resolve(v) )
				})
			}
		}
	}()
