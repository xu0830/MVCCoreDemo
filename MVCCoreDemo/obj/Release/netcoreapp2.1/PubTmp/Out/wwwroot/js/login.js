var Main = {
    data() {
        var _this = this;
        var validateUserName = (rule, value, callback) => {
            if (value === '') {
                callback(new Error('请输入用户名'));
            } else {
                callback();
            }
        };
        var validatePassword = (rule, value, callback) => {
            if (value === '') {
                callback(new Error('请输入密码'));
            } else {
                callback();
            }
        };
        var validateSlide = (rule, value, callback) => {
            if (!_this.validationComplete) {
                callback(new Error('请完成滑块验证'));
            } else {
                callback();
            }
        };
        return {
            loginForm: {
                userName: '',
                password: '',
                slideVerify: false,
                remember: false,
            },
            loginRules: {
                userName: [
                    { validator: validateUserName, trigger: 'blur' }
                ],
                password: [
                    { validator: validatePassword, trigger: 'blur' }
                ],
                slideVerify: [
                    { validator: validateSlide, trigger: 'blur' }
                ]
            },
            sliderStatus: "",
            isSliderDragActive: false,
            IsSliderActive: false,
            labelPosition: 'right',
            picBlockFocus: false,
            picBlockBlur: true,
            dragX: 0,
            picDragX: 0,
            initX: 0,
            randomX: 0,
            randomY: 0,
            border_color: '#1991fa',
            imgData: {},
            validationComplete: false,
            validationError: false
        }
    },
    computed: {
        slideCtrlObj: function () {
            return {
                sliderCtrl: true,
            }
        }
    },
    created: function () {
        let encrypt = new JSEncrypt();
        this.verifyPicDraw();
    },
    methods: {
        onSubmit: function (formName) {
            let _this = this;
            this.$refs[formName].validate((valid) => {
                if (valid) {
                    let encrypt = new JSEncrypt();
                    _this.$http.post('/Login/GetRsaPublicKey', {}).then(function (data) {
                        if (data.body.code == 200) {
                            _this.$http.post('/Login/Login', {
                                UserName: _this.loginForm.userName,
                                Password: encrypt.encrypt(_this.loginForm.password),
                                Remember: _this.loginForm.remember
                            }).then(function (data) {
                                if (data.body.code == 200) {
                                    window.location.href = "/Home/Index"
                                }
                            });
                            this.$refs[formName].resetFields();
                        }
                    });
                } else {
                    return false;
                }
            });
        },
        sliderClick: function (event) {
           
            if (!this.validationComplete) {
                this.isSliderDragActive = true;
                this.initX = event.pageX;
                this.sliderStatus = "sliderActive";
                this.picBlockFocus = true;
            }
        },
        sliderActive: function () {
            if (!this.validationComplete && !this.validationError) {
                this.sliderStatus = "sliderActive";
            } 
        },
        sliderBlur: function () {
            if (this.isSliderDragActive) {
                this.sliderStatus = "sliderActive";
                this.picBlockFocus = true;
            }
            else if (this.sliderStatus === "sliderActive") {
                this.sliderStatus = "";
                this.picBlockFocus = false;
            }
        },
        mouseUp: function (event) {
            if (this.isSliderDragActive) {
                this.isSliderDragActive = false;

                let flagCode = 0;

                this.$http.post('/Login/CheckVerifyPic', {
                    PointX: this.picDragX
                }).then(function (data) {
                    
                    flagCode = data.body.code;
                    if (flagCode == 200) {
                        this.validationComplete = true;
                        this.sliderStatus = "validComplete";
                        this.picBlockFocus = false;
                    } else {
                        this.validationError = true;
                        this.validationComplete = false;
                        this.sliderStatus = "validError";
                        this.picBlockFocus = true;
                        let _this = this;
                        setTimeout(function () {
                            _this.validationError = false;
                            _this.dragX = 0;
                            _this.sliderStatus = "";
                            _this.verifyPicDraw();
                        }, 500);
                    }
                    this.$refs.loginForm.validateField('slideVerify');
                }); 
   
            }
        },
        picContainerFocus: function (event) {
            this.picBlockFocus = true;
        },
        sliderMousemove: function (event) {
            if (this.isSliderDragActive) {
                var dragDistance = event.pageX - this.initX < 0 ? 0 : event.pageX - this.initX > 210 ? 210 : event.pageX - this.initX;
                this.dragX = dragDistance;
                
                this.picDragX = dragDistance > 210 - 8 * 1.8 ? 210 - 8 * 1.8 : dragDistance;
                vm.moveJigsaw(vm.imgData, this.picDragX, vm.randomY, 8, 16);
            }
        },
        verifyPicShow: function () {
            if (!this.validationComplete) {
                let _this = this;
                _this.picBlockBlur = false;
                if (!_this.picBlockFocus) {
                    setTimeout(function () {
                        if (!_this.picBlockFocus && !_this.picBlockBlur) {
                            _this.picBlockFocus = true;
                        }
                    },200);
                }
            }
        },
        verifyPicHide: function () {
            this.picBlockBlur = true;
            var _this = this;
            if (_this.picBlockFocus && !this.isSliderDragActive) {
                setTimeout(function () {
                    if (_this.picBlockFocus && _this.picBlockBlur) {
                        _this.picBlockFocus = false;
                    }
                }, 600);
            }
            
        },
        verifyPicDraw: function () {
            var img = new Image();
            var side = 16;
            var radius = 8;
            var sideLength = 2 * side + 0.5 * radius * (4 + 1.8);
            var maxX = 250 - sideLength;
            var minX = 2 * sideLength;

            var maxY = 140 - sideLength;
            var minY = radius + 0.5 * 1.8 * radius;

            this.$http.get('/Login/GetVerifyPicPosition').then(function (res) {
                let data = res.body;
                this.randomX = data.pointX;
                this.randomY = data.pointY;
                var _this = this;
                img.onload = function () {
                    var canvas_bot = document.getElementById('canvas_bot').getContext('2d');
                    canvas_bot.drawImage(img, 0, 0, 250, 140);
                    canvas_bot.save();
                    _this.drawJigsaw(canvas_bot, _this.randomX, _this.randomY, side, radius);
                    canvas_bot.stroke();
                    canvas_bot.clip();
                    _this.imgData = canvas_bot.getImageData(_this.randomX, _this.randomY - radius * (1 + 0.5 * 1.8),
                        2 * side + radius * (2 + 0.5 * 1.8),
                        2 * side + radius * (2 + 0.5 * 1.8));
                    canvas_bot.fillStyle = 'rgba(17, 17, 17, 0.92)';
                    canvas_bot.fillRect(0, 0, 250, 140);

                    canvas_bot.restore();

                    var canvas_move = document.getElementById('canvas_move').getContext('2d');
                    canvas_move.clearRect(0, 0, 250, 140);

                    canvas_move.putImageData(_this.imgData, 1, _this.randomY - radius * (1 + 0.5 * 1.8));
                    canvas_move.globalCompositeOperation = "destination-in";
                    canvas_move.save();
                    _this.drawJigsaw(canvas_move, 0, _this.randomY, side, radius);
                    canvas_move.fillStyle = 'green';
                    canvas_move.fill();
                    canvas_move.clip();
                    canvas_move.restore();
                }
                let pic_index = Math.floor(Math.random() * 10 + 1);
                img.src = `/images/pic_${pic_index}.jpg`;
            });
        },
        drawJigsaw: function (canvasObj, randomX, randomY, side, radius) {
            canvasObj.strokeStyle = 'rgba(205, 207, 206, 0.9)';
            canvasObj.beginPath();
            canvasObj.moveTo(randomX, randomY);
            canvasObj.lineTo(randomX + side, randomY);
            canvasObj.arc(randomX + side + 0.5 * radius, randomY - radius * 0.5 * 1.8, radius, 2 / 3 * Math.PI, 1 / 3 * Math.PI, false);
            canvasObj.lineTo(randomX + 2 * side + radius, randomY);
            canvasObj.lineTo(randomX + 2 * side + radius, randomY + side);
            canvasObj.arc(randomX + 2 * side + radius + radius * 0.5 * 1.8, randomY + side + 0.5 * radius, radius, 7 / 6 * Math.PI, 5 / 6 * Math.PI, false);
            canvasObj.lineTo(randomX + 2 * side + radius, randomY + 2 * side + radius);
            canvasObj.lineTo(randomX, randomY + 2 * side + radius);
            canvasObj.lineTo(randomX, randomY + side + radius);
            canvasObj.arc(randomX + radius * 0.5 * 1.8, randomY + side + 0.5 * radius, radius, 5 / 6 * Math.PI, 7 / 6 * Math.PI, true);
            canvasObj.lineTo(randomX, randomY);
        },
        moveJigsaw: function (imgData, dragX, randomY, radius, side) {
            var ctx_bot = document.getElementById('canvas_bot').getContext('2d');
            
            var ctx_move = document.getElementById('canvas_move').getContext('2d');
            ctx_move.clearRect(0, 0, 250, 140);
            ctx_move.putImageData(imgData, dragX, randomY - radius * (1 + 0.5 * 1.8)+1);
            ctx_move.globalCompositeOperation = "destination-in";
            ctx_move.save();
            vm.drawJigsaw(ctx_move, dragX, randomY, side, radius);
            ctx_move.closePath();
            ctx_move.fillStyle = 'green';
            ctx_move.fill();
            ctx_move.clip();
            ctx_move.restore();
        }
    }
}
var Ctor = Vue.extend(Main);
var vm = new Ctor().$mount('#app')
